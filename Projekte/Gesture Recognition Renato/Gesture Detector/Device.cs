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
    public class Device
    {
        private KinectSensor Dev;

        /**
         * We keep track of the sensors movement. If its moving, there wont be any gesture recognition.
         */
        private Vector4 lastAcceleration;

        /**
         * List of persons which are currently tracked
         */
        private List<Person> persons;

        /**
         * Expoloration Candidates - All persons which currently dont have a skeleton
         * We keep them for 5 seconds to prevent glitches in gesture recognition.
         */
        private Dictionary<long, Person> explorationCandidates;

        /**
         * Instatiation of the kinect wrapper. It handles frame events, creates new
         * persons and decides who's currently active.
         */
        public Device()
        {
            // Receive KinectSensor instance from physical device
            Dev = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            initialize();
        }

        private void initialize()
        {
            lastAcceleration = new Vector4();
            persons = new List<Person>();
            explorationCandidates = new Dictionary<long, Person>();
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            Dev.Dispose();
        }

        /**
         * Returns the currently active person. If no person is active, null is returned.
         */
        public List<Person> GetActivePerson()
        {
            return persons.FindAll(x => x.Active == true);
        }

        public List<Person> GetAll()
        {
            return persons;
        }

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
                //TODO Remove this to increase Performante
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

        protected void handleNewSkeletons(List<SmothendSkeleton> skeletonList)
        {
            

            // Remove persons older than 5 seconds from dictionary
            long allowedAge = CurrentMillis.Millis - 5000;
            List<KeyValuePair<long, Person>> tempList = new List<KeyValuePair<long,Person>>();
            tempList.AddRange(explorationCandidates.Where(x => x.Key > allowedAge).ToList());
            foreach (KeyValuePair<long, Person> item in tempList)
            {
                //avoiding memory leak
                item.Value.OnWave -= personWaved;
                explorationCandidates.Remove(item.Key);
            }

            /**
                * Es gibt 3 verschiedene Möglichkeiten den aktuellen Status zu beschreiben:
                * - Alle Personen hatten schon ein Skelett. Die Zuweisung muss neu erfolgen
                * - Es gibt weniger Skelette als Personen. übrige Person muss gelöscht werden
                * - Es gibt mehr Skelette als Personen. Eine neue Person muss erstellt werden
                */
            Match bestMatch = new Match();

            if (skeletonList.Count < persons.Count) // eine Person ging aus dem Bild
            {
                List<Person> personList = new List<Person>(); // Kopiere Personen für Matchingverfahren
                personList.AddRange(persons);
                foreach (SmothendSkeleton s in skeletonList) // für jedes Skelett wird der beste Match gesucht
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
                    bestMatch.Person.AddSkeleton(bestMatch.Skeleton); // weise neues Skelett zu
                    personList.Remove(bestMatch.Person);
                }
                // Lösche übriggebliebene Personen, da sie kein Skelett mehr haben
                foreach (Person p in personList)
                {
                    persons.Remove(p);
                    explorationCandidates.Add(CurrentMillis.Millis, p);//add person to cache
                    PersonLost(this, new PersonDisposedEventArgs(p));
                }
            }
            else // eine Person kam ins Bild
            {
                List<Person> personList = new List<Person>(); // Kopiere Personen für Matchingverfahren
                personList.AddRange(persons);
                foreach (Person p in personList) // für jede Person wird der beste Match gesucht
                {
                    bestMatch.Value = double.MaxValue;
                    double v;
                    foreach (SmothendSkeleton s in skeletonList)
                    {
                        v = p.Match(s);
                        if (v < bestMatch.Value)
                        {
                            bestMatch.Value = v;
                            bestMatch.Person = p;
                            bestMatch.Skeleton = s;
                        }
                    }
                    bestMatch.Person.AddSkeleton(bestMatch.Skeleton); // weise neues Skelett zu
                    skeletonList.Remove(bestMatch.Skeleton);
                }
                //Match to Cache
                List<SmothendSkeleton> skeletonsToRemove = new List<SmothendSkeleton>();
                foreach (SmothendSkeleton s in skeletonList)
                {
                    bestMatch.Value = double.MaxValue;
                    double v;
                    foreach (long l in explorationCandidates.Keys)
                    {
                        Person p = explorationCandidates[l];
                        v = p.Match(s);
                        if (v < bestMatch.Value)
                        {
                            bestMatch.Value = v;
                            bestMatch.Person = p;
                            bestMatch.Skeleton = s;
                            bestMatch.Key = l;
                        }
                    }
                    if (bestMatch.Value < 0.5) // match valid
                    {
                        //set person active again
                        persons.Add(bestMatch.Person);
                        registerWave(bestMatch.Person);
                        bestMatch.Person.AddSkeleton(bestMatch.Skeleton);//give the new Skeleton
                        explorationCandidates.Remove(bestMatch.Key);
                        skeletonsToRemove.Add(bestMatch.Skeleton);
                    }
                }
                foreach (SmothendSkeleton ske in skeletonsToRemove)
                {
                    skeletonList.Remove(ske);
                }
                //new person for unmatched skeletons
                skeletonsToRemove.Clear();
                // erstelle für übrige Skelette jeweils Personen
                foreach (SmothendSkeleton s in skeletonList)
                {
                    Person p = new Person(this);
                    p.AddSkeleton(s);
                    persons.Add(p);
                    registerWave(p);
                    if (NewPerson != null)
                    {
                        NewPerson(this, new NewPersonEventArgs(p));
                    }
                }
            }
        }


        /**
         * Begin detecting if a person has waved
         */
        private void registerWave(Person person)
        {
            person.OnWave += personWaved;
        }

        /**
         * Returns the current acceleration of the physical device.
         */
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

        private void personWaved(object sender, GestureEventArgs e)
        {
            Person p = ((Person)sender);
            // set active if ther isn't another active person
            if (!p.Active && persons.Find(x => x.Active==true) == null)
            {
                p.Active = true;
                PersonActive(this ,new ActivePersonEventArgs(p));
            }
        }

        private class Match
        {
            public double Value {get; set; }
            public Person Person {get; set; }
            public SmothendSkeleton Skeleton {get; set; }
            public long Key { get; set; }
        };

        #region Events

        public event EventHandler<ActivePersonEventArgs> PersonActive;
        //public delegate void ActivePersonHandler<TEventArgs> (object source, TEventArgs e) where TEventArgs:EventArgs;

        public event EventHandler<NewPersonEventArgs> NewPerson;
        //public delegate void PersonHandler<TEventArgs>(object source, TEventArgs e) where TEventArgs : EventArgs;

        public event EventHandler<AccelerationEventArgs> Accelerated;

        public event EventHandler<PersonDisposedEventArgs> PersonLost;

        #endregion
    }
}
