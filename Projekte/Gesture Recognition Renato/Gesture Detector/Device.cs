using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Diagnostics;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures.Wave;
using MF.Engineering.MF8910.GestureDetector.Exceptions;
using System.Runtime.CompilerServices;

namespace MF.Engineering.MF8910.GestureDetector.DataSources
{
    /// <summary>
    /// Mapper for the KinectSensor. 
    /// Implements person recognition, login and activity.
    /// 
    /// Restrictions:
    /// - If the physical device is moving, persons currenty aren't tracked</summary>
    public class Device
    {
        /// <summary>
        /// How long we keep invisible persons in cache [milliseconds].</summary>
        private int CACHE_MERCY_TIME = 5000;

        private KinectSensor Dev;

        /// <summary>
        /// We keep track of the sensors movement. If its moving, there wont be any gesture recognition.</summary>
        private Vector4 lastAcceleration;

        /// <summary>
        /// List of persons which are currently tracked</summary>
        private List<Person> trackedPersons;

        /// <summary>
        /// Expiration Candidates - All persons which currently dont have a skeleton
        /// We keep them for 5 seconds to prevent glitches in gesture recognition.</summary>
        private Dictionary<long, Person> expirationCandidates;

        /// <summary>
        /// Initialization of the kinect wrapper and the physical device. 
        /// It handles frame events, creates new persons and decides who's currently active.</summary>
        public Device()
        {
            // Receive KinectSensor instance from physical device
            Dev = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            initialize();
        }

        /// <summary>
        /// Register relevant device events and begin streaming</summary>
        private void initialize()
        {
            lastAcceleration = new Vector4();
            trackedPersons = new List<Person>();
            expirationCandidates = new Dictionary<long, Person>();
            Dev.SkeletonStream.Enable(); // Begin to capture skeletons
            Dev.SkeletonFrameReady += OnNewSkeletons; // Register on any new skeletons
        }

        public Device(string uniqueId) // get a specified Kinect by its ID
        {
            foreach (KinectSensor ks in KinectSensor.KinectSensors)
            {
                if (ks.UniqueKinectId == uniqueId)
                {
                    Dev = ks;
                    initialize();
                }
            }
        }

        public KinectStatus Status { get { return Dev.Status; } }

        /// <summary>
        /// Start receiving skeletons.</summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (!Dev.IsRunning)
            {
                Dev.Start();
            }
            if (!Dev.IsRunning)
            {
                throw new DeviceErrorException("Device did not start") { Status = Dev.Status } ;
            }
        }

        /// <summary>
        /// Stop receiving skeletons.</summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (Dev.IsRunning)
            {
                Dev.Stop();
            }
            if (Dev.IsRunning)
            {
                throw new DeviceErrorException("Device did not stop") { Status = Dev.Status };
            }
        }

        /// <summary>
        /// Disopse device resources.</summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            this.Stop();
            Dev.Dispose();
        }

        /// <summary>
        /// Should always return the only currently active person. It would theoretically
        /// be possible to serve multiple active persons, but its not implemented.</summary>
        /// <returns>
        /// Returns the currently active person. If no person is active, null is returned.</returns>
        public List<Person> GetActivePerson()
        {
            return trackedPersons.FindAll(x => x.Active == true);
        }

        /// <summary>
        /// Get all persons tracked by the Kinect.</summary>
        /// <returns>
        /// List of tracked persons</returns>
        public List<Person> GetAll()
        {
            return trackedPersons;
        }

        /// <summary>
        /// Event for receiving new skeletons. Its called on every frame delivered
        /// by the KinectSensor (every 30ms) and decides if a frame is valid or will
        /// be ignored. If a frame is valid, we read all tracked skeletons from it
        /// and smooth it. After that, the skeletons are analyzed (handleSkeletons).</summary>
        /// <param name="source">
        /// The source is probably the KinectSensor</param>
        /// <param name="e">
        /// Resource to get the current frame from.</param>
        protected void OnNewSkeletons(object source, SkeletonFrameReadyEventArgs e)
        {
            double diff = getAccelerationDiff();
            if ((diff > 0.1 || diff < -0.1) && Accelerated != null) 
            {
                // Fire event for indicating unstable device
                Accelerated(this, new AccelerationEventArgs(diff));
            }
            else
            {
                SkeletonFrame skeletonFrame = e.OpenSkeletonFrame();
                //TODO Remove this to increase Performance
                if (skeletonFrame != null)
                {
                    // TODO ev Performance Problem because of reinstantiating Array (see profiling)
                    Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    // Copy actually tracked skeletons to a list which is easier to work with.
                    List<SmothendSkeleton> skeletonList = new List<SmothendSkeleton>();
                    foreach (Skeleton ske in skeletons)
                    {
                        if (ske.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            skeletonList.Add(new SmothendSkeleton(ske, skeletonFrame.Timestamp));
                        }
                    }
                    skeletonFrame.Dispose();
                    handleNewSkeletons(skeletonList);
                }
            }
        }

        /// <summary>
        /// Analyze skeletons and assign them to existing persons. This is done
        /// with a naive matching technique:
        ///   Compare every new skeletons hip center with every cached persons 
        ///   last skeletons hip center.
        /// After a person got a new skeletons, his timestamp in the cache is renewed.</summary>
        /// <param name="skeletonsToMatch">
        /// List of smoothend skeletons</param>
        protected void handleNewSkeletons(List<SmothendSkeleton> skeletonsToMatch)
        {
            // Remove persons older than 5 seconds from dictionary
            long allowedAge = CurrentMillis.Millis - CACHE_MERCY_TIME;
            List<KeyValuePair<long, Person>> tempList = new List<KeyValuePair<long,Person>>();
            tempList.AddRange(expirationCandidates.Where(x => x.Key > allowedAge).ToList());
            foreach (KeyValuePair<long, Person> item in tempList)
            {
                // avoiding memory leak
                item.Value.OnWave -= personWaved;
                expirationCandidates.Remove(item.Key);
            }

            /**
             * There are 3 possibilities to describe the current status
             * - Every person had a skeleton. We just need to assign new ones.
             * - There are less skeletons than persons. At least one person doesnt exist 
             *   anymore and has to be deleted (it still exists in the cache).
             * - There are more skeletons than persons. Maybe we have to rehabilitate 
             *   a cached person or to create a new one.
             */
            Match bestMatch = new Match();

            /**
             * A person went out of sight
             */
            if (skeletonsToMatch.Count < trackedPersons.Count) 
            {
                List<Person> personList = new List<Person>(); // copy currently tracked persons for the matching algorithm
                personList.AddRange(trackedPersons);
                foreach (SmothendSkeleton s in skeletonsToMatch) // search best match for every SKELETON
                {
                    bestMatch.Value = double.MaxValue;
                    double v;
                    foreach (Person p in personList)
                    {
                        v = p.Match(s);
                        if (v < bestMatch.Value)
                        {
                            bestMatch.Value = v;
                            bestMatch.Person = p;
                            bestMatch.Skeleton = s;
                        }
                    }
                    bestMatch.Person.AddSkeleton(bestMatch.Skeleton); // Assign new skeleton
                    personList.Remove(bestMatch.Person);
                }

                // Delete remaining persons since they don't have a skeleton anymore (they still exists in cache)
                foreach (Person p in personList)
                {
                    trackedPersons.Remove(p);
                    expirationCandidates.Add(CurrentMillis.Millis, p); // Add person to cache
                    if (PersonLost != null) 
                    {
                        PersonLost(this, new PersonDisposedEventArgs(p));
                    }
                }
            }

            /**
             * A person came in sight or there were no changes
             */
            else
            {
                List<Person> personList = new List<Person>(); // copy currently tracked persons for the matching algorithm
                personList.AddRange(trackedPersons);
                foreach (Person p in personList) // search best match for every PERSON
                {
                    bestMatch.Value = double.MaxValue;
                    double v;
                    foreach (SmothendSkeleton s in skeletonsToMatch)
                    {
                        v = p.Match(s);
                        if (v < bestMatch.Value)
                        {
                            bestMatch.Value = v;
                            bestMatch.Person = p;
                            bestMatch.Skeleton = s;
                        }
                    }
                    bestMatch.Person.AddSkeleton(bestMatch.Skeleton); // assign new skeleton
                    skeletonsToMatch.Remove(bestMatch.Skeleton);
                }

                /**
                 * After we matched the tracked persons, we try to match the 
                 * remaining skeletons to the cached persons.
                 */
                List<SmothendSkeleton> skeletonsToRemove = new List<SmothendSkeleton>();
                foreach (SmothendSkeleton s in skeletonsToMatch)
                {
                    bestMatch.Value = double.MaxValue;
                    double v;
                    foreach (long l in expirationCandidates.Keys)
                    {
                        Person p = expirationCandidates[l];
                        v = p.Match(s);
                        if (v < bestMatch.Value)
                        {
                            bestMatch.Value = v;
                            bestMatch.Person = p;
                            bestMatch.Skeleton = s;
                            bestMatch.Timestamp = l;
                        }
                    }
                    if (bestMatch.Value < 0.5) // match valid
                    {
                        // There was a match. Person will be rehabilitated and tracked again.
                        trackedPersons.Add(bestMatch.Person);
                        registerWave(bestMatch.Person);
                        bestMatch.Person.AddSkeleton(bestMatch.Skeleton); // Assign the new Skeleton
                        expirationCandidates.Remove(bestMatch.Timestamp);
                        skeletonsToRemove.Add(bestMatch.Skeleton);
                    }
                }
                foreach (SmothendSkeleton ske in skeletonsToRemove)
                {
                    skeletonsToMatch.Remove(ske);
                }

                /**
                 * We didn't find neighter a match for tracked persons nor for cached persons.
                 * -> We will create new persons for all unmatched skeletons.
                 */
                foreach (SmothendSkeleton s in skeletonsToMatch)
                {
                    Person p = new Person(this);
                    p.AddSkeleton(s);
                    trackedPersons.Add(p);
                    registerWave(p);
                    if (NewPerson != null)
                    {
                        NewPerson(this, new NewPersonEventArgs(p));
                    }
                }
            }
        }


        /// <summary>
        /// Begin detecting if a person is waving.</summary>
        /// <param name="p">Person to monitor</param>
        private void registerWave(Person p)
        {
            p.OnWave += personWaved;
        }

        /// <summary>
        /// Returns the current acceleration of the physical device.</summary>
        /// <returns>
        /// True if the physical device is moving, false otherwise.</returns>
        private double getAccelerationDiff()
        {
            double diff = 0; // Difference between last accelerometer readings and actual readings
            diff += (Dev.AccelerometerGetCurrentReading().W - lastAcceleration.W);
            diff += (Dev.AccelerometerGetCurrentReading().X - lastAcceleration.X);
            diff += (Dev.AccelerometerGetCurrentReading().Y - lastAcceleration.Y);
            diff += (Dev.AccelerometerGetCurrentReading().Z - lastAcceleration.Z);
            lastAcceleration = Dev.AccelerometerGetCurrentReading();
            return diff;
        }

        /// <summary>
        /// If a person waved, we need to activate her for further gesture recognition.</summary>
        /// <param name="sender">
        /// This device class</param>
        /// <param name="e">
        /// Gesture details</param>
        private void personWaved(object sender, GestureEventArgs e)
        {
            Person p = ((Person)sender);
            // Set person active if there isn't another active person
            if (!p.Active && trackedPersons.Find(x => x.Active==true) == null)
            {
                p.Active = true;
                if (PersonActive != null)
                {
                    PersonActive(this ,new ActivePersonEventArgs(p));
                }
            }
        }

        /// <summary>
        /// Struct for saving details of the matching algoritm.</summary>
        private class Match
        {
            /// <summary>
            /// Distance of the two skeletons hip centers</summary>
            public double Value {get; set; }
            /// <summary>
            /// Person of whom the skeleton was matched</summary>
            public Person Person {get; set; }
            /// <summary>
            /// Compared skeleton</summary>
            public SmothendSkeleton Skeleton {get; set; }
            /// <summary>
            /// The matches timestamp</summary>
            public long Timestamp { get; set; }
        };

        #region Events

        /// <summary>
        /// Called when a person got active</summary>
        public event EventHandler<ActivePersonEventArgs> PersonActive;
        //public delegate void ActivePersonHandler<TEventArgs> (object source, TEventArgs e) where TEventArgs:EventArgs;

        /// <summary>
        /// Called when the device recognized a new person</summary>
        public event EventHandler<NewPersonEventArgs> NewPerson;
        //public delegate void PersonHandler<TEventArgs>(object source, TEventArgs e) where TEventArgs : EventArgs;

        /// <summary>
        /// Called when the Kinect was moving.</summary>
        public event EventHandler<AccelerationEventArgs> Accelerated;

        /// <summary>
        /// Called when a person was lost.</summary>
        public event EventHandler<PersonDisposedEventArgs> PersonLost;

        #endregion
    }
}
