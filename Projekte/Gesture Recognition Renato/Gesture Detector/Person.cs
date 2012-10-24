using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSources;
using GestureEvents;
using Gesture_Detector;

namespace DataSources
{
    public class Person
    {
        private bool active;
        private SmothendSkeleton[] skeletons;
        private int[] time;
        private int index;
        private Device dev;
        private int id;

        public Person(Device d)
        {
            Random r = new Random();
            skeletons = new SmothendSkeleton[10];
            time = new int[10];
            index = 0;
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
            if (index == 9)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            skeletons[index]=e.GetSkeleton(this);
            time[index] = DateTime.Now.Millisecond;
            NewSkeleton(this, new NewSkeletonEventArg(e.GetSkeleton(this)));
        }

        public SmothendSkeleton GetSkeleton
        {
            get { return skeletons[index]; }
        }

        public bool SendEventsWhenPassive { get; set; }

        public SmothendSkeleton GetLastSkeleton(int i)
        {
            return skeletons[(index - i + 10) % 10];
        }

        public int MillisBetweenFrames(int first, int second)
        {
            return time[(index - second + 10) % 10] - time[(index - first + 10) % 10];
        }

        public bool Active { get; set; }

        public int ID { get { return id; } }

        public bool SendEventsWhenPassive { get; set; }

        public override bool Equals(object obj)
        {
            return id == ((Person)obj).ID;
        }

        public event EventHandler<NewSkeletonEventArg> NewSkeleton;
        public event EventHandler<PersonPassiveEventArgs> PersonPassive;
        public event EventHandler<ActivePersonEventArgs> PersonActive;
        public event EventHandler<PersonDisposedEventArgs> PersonDisposed;

        public event EventHandler OnWave;
        //public event EventHandler blablub;
        //public event EventHandler etc;
    }
}
