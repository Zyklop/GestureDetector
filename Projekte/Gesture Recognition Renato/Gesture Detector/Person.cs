using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSources;
using GestureEvents;
using Gesture_Detector;
using System.Diagnostics;

namespace DataSources
{
    public class Person
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
            dev.NewSkeleton += PickUpSkeleton;
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

        void PickUpSkeleton(object src, SkeletonsReadyEventArg e)
        {
            skeletons.Add(DateTime.Now.Millisecond,e.GetSkeleton(this));
            if (skeletons.Count > 10)
            {
                skeletons.Remove(skeletons.Min().Key);
            }
            NewSkeleton(this, new NewSkeletonEventArg(e.GetSkeleton(this)));
        }

        public SmothendSkeleton CurrentSkeleton
        {
            get { return skeletons.First().Value; }
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
