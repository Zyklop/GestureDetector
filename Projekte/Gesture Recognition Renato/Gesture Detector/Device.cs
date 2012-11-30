using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Diagnostics;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures.Wave;

namespace MF.Engineering.MF8910.GestureDetector.DataSources
{
    public class PersonCacheEntry
    {
        public long Timestamp { get; set; }
        public Person Person { get; set; }

        private bool isValid = true;
        public bool IsValid()
        {
            return isValid;
        }
        public void Invalidate()
        {
            isValid = false;
        }
        public void Revalidate()
        {
            isValid = true;
        }
    }

    public class Device
    {
        private KinectSensor Dev;

        /**
         * We keep track of the sensors movement. If its moving, there wont be any gesture recognition.
         */
        private Vector4 lastAcceleration;

        /**
         * Expoloration Candidates - All persons which currently dont have a skeleton
         * We keep them for 5 seconds to prevent glitches in gesture recognition.
         */
        private List<PersonCacheEntry> personCache;

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
            personCache = new List<PersonCacheEntry>();
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

        public bool Start()
        {
            if (!Dev.IsRunning)
            {
                Dev.Start();
            }
            return Dev.IsRunning;
        }

        public bool Stop()
        {
            if (Dev.IsRunning)
            {
                Dev.Stop();
            }
            return Dev.IsRunning;
        }

        public void Dispose()
        {
            Dev.Dispose();
        }

        /**
         * Returns the currently active person. If no person is active, null is returned.
         */
        public Person GetActivePerson()
        {
            return personCache.Find(x => x.Person.Active == true).Person;
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
                    skeletonFrame.Dispose();
                    handleNewSkeletons(skeletons);
                }
            }
        }

        protected void handleNewSkeletons(Skeleton[] skeletons)
        {
            // Copy actually tracked skeletons to a list which is easier to work with.
            List<SmothendSkeleton> skeletonList = new List<SmothendSkeleton>();
            foreach (Skeleton ske in skeletons)
            {
                if (ske.TrackingState == SkeletonTrackingState.Tracked)
                {
                    skeletonList.Add(new SmothendSkeleton(ske));
                }
            }

            // Remove persons older than 5 seconds from cache
            long allowedAge = DateTime.Now.Ticks - 5000;
            IEnumerator<PersonCacheEntry> iter = personCache.GetEnumerator();
            while(iter.MoveNext()) {
                if (iter.Current.Timestamp > allowedAge) {
                    iter.Current.Person.OnWave -= personWaved;
                    personCache.Remove(iter.Current);
                }
            }

            /**
                * Es gibt 3 verschiedene Möglichkeiten den aktuellen Status zu beschreiben:
                * - Alle Personen hatten schon ein Skelett. Die Zuweisung muss neu erfolgen
                * - Es gibt weniger Skelette als Personen. übrige Person muss gelöscht werden
                * - Es gibt mehr Skelette als Personen. Eine neue Person muss erstellt werden
                */

            /**
             * Search best matching person for each skeleton in the list
             */
            foreach (SmothendSkeleton s in skeletonList)
            {
                //PersonCacheEntry minMatch = personCache.Min(x => x.Person.Match(s));
                PersonCacheEntry minMatch = null;
                double bestDiff = double.MaxValue;
                foreach (PersonCacheEntry c in personCache)
                {
                    double v = c.Person.Match(s);
                    if (v < bestDiff)
                    {
                        minMatch = c;
                    }
                }

                // Got new person or detected glitch
                if (!minMatch.IsValid()) 
                {
                    if (bestDiff < 0.5) { // Glitch detected. Person is still active
                        minMatch.Revalidate();
                    } else { // New person detected
                        minMatch = new PersonCacheEntry();
                        minMatch.Person = new Person(this);
                        registerWave(minMatch.Person);
                        if (NewPerson != null)
                        {
                            NewPerson(this, new NewPersonEventArgs(minMatch.Person));
                        }
                    }
                }

                minMatch.Timestamp = DateTime.Now.Ticks;
                minMatch.Person.AddSkeleton(s);
            }

            // TODO invalidate who didnt get a skeleton

            // TODO delete entries after 5s
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
            if (!p.Active && personCache.Find(x => x.Person.Active == true) == null)
            {
                p.Active = true;
                PersonActive(this ,new ActivePersonEventArgs(p));
            }
        }

        #region Events

        public event EventHandler<ActivePersonEventArgs> PersonActive;
        //public delegate void ActivePersonHandler<TEventArgs> (object source, TEventArgs e) where TEventArgs:EventArgs;

        public event EventHandler<NewPersonEventArgs> NewPerson;
        //public delegate void PersonHandler<TEventArgs>(object source, TEventArgs e) where TEventArgs : EventArgs;

        public event EventHandler<AccelerationEventArgs> Accelerated;

        #endregion
    }
}
