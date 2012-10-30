using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSources;
using GestureEvents;
using Gesture_Detector;
using System.Diagnostics;
using Microsoft.Kinect;
using Conditions;

namespace DataSources
{
    public class Person: IComparable
    {
        private bool active;
        private SortedDictionary<int, SmothendSkeleton> skeletons;
        private Device dev;
        private int id;

        public Person(Device d)
        {
            Random r = new Random();
            skeletons = new SortedDictionary<int, SmothendSkeleton>(new DescendingTimeComparer<int>());
            dev = d;
            id = r.Next();
        }

        //public StaticSmothendSkeleton getStaticSkeleton()
        //{
        //    return null;
        //}

        //public MovingSmothendSkeleton getMovingSkeleton()
        //{
        //    return null;
        //}

        //public StaticSmothendSkeleton getLastStaticSkeleton(int frames)
        //{
        //    return null;
        //}

        //public MovingSmothendSkeleton getLastMovingSkeleton(int frames)
        //{
        //    return null;
        //}

        public void AddSkeleton(SmothendSkeleton ss)
        {
            skeletons.Add(DateTime.Now.Millisecond,ss);
            if (skeletons.Count > 10)
            {
                skeletons.Remove(skeletons.ElementAt(9).Key);
            }
            if (NewSkeleton != null)
            {
                NewSkeleton(this, new NewSkeletonEventArg(ss));
            }
        }

        public SmothendSkeleton CurrentSkeleton
        {
            get 
            {
                if (skeletons.Count  == 0)
                {
                    return new SmothendSkeleton(new Microsoft.Kinect.Skeleton());
                }
                return skeletons.First().Value; 
            }
        }

        public SmothendSkeleton GetLastSkeleton(int i)
        {
            return skeletons.ElementAt(i).Value;
        }

        public int MillisBetweenFrames(int first, int second)
        {
            int diff = skeletons.ElementAt(second).Key - skeletons.ElementAt(first).Key;
            Debug.WriteLineIf(diff < 0, "Time Difference negative in MillisBetweenFrame");
            return diff;
        }

        public bool Active { get; set; }

        public int ID { get { return id; } }

        public bool SendEventsWhenPassive { get; set; }

        public override bool Equals(object p)
        {
            return GetHashCode().Equals(((Person)p).GetHashCode());
        }

        public override int GetHashCode()
        {
            return ID;
        }

        public int CompareTo(object other)
        {
            SkeletonPoint currentRoot = this.CurrentSkeleton.GetPosition(JointType.HipCenter);
            SkeletonPoint otherRoot;

            if (other is SmothendSkeleton) 
            {
                otherRoot = ((SmothendSkeleton)other).GetPosition(JointType.HipCenter);
            }
            else if (other is Person)
            {
                otherRoot = ((Person)other).CurrentSkeleton.GetPosition(JointType.HipCenter);
            }
            else
            {
                throw new ArgumentException("Neighter Person nor SmothendSkeleton given as argument!");
            }

            double COMPARING_SCOPE = 0.1; // Alle mit weniger als 10cm Abstand wird als gleich wahrgenommen (return 0)
            return Math.Abs((int)(SkeletonMath.DistanceBetweenPoints(currentRoot, otherRoot) / COMPARING_SCOPE));
        }

        public event EventHandler<NewSkeletonEventArg> NewSkeleton;
        public event EventHandler<PersonPassiveEventArgs> PersonPassive;
        public event EventHandler<ActivePersonEventArgs> PersonActive;
        public event EventHandler<PersonDisposedEventArgs> PersonDisposed;

        public event EventHandler OnWave;
        //public event EventHandler blablub;
        //public event EventHandler etc;

        private class DescendingTimeComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T first, T second)
            {
                if (((int)((object)first)) < 1000 &&  ((int)((object)second)) > (Int32.MaxValue - 1000))
                {
                    return 1;
                }
                if (((int)((object)first)) < ((int)((object)second)))
                {
                    return 1;
                }
                if (((int)((object)first)) > ((int)((object)second)))
                {
                    return -1;
                }
                return 0;
            }
        }
    }
}
