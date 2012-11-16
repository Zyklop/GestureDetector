using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Kinect;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Tools;
using MF.Engineering.MF8910.GestureDetector.Gestures.Wave;
using MF.Engineering.MF8910.GestureDetector.Gestures.Zoom;
using MF.Engineering.MF8910.GestureDetector.Gestures.Swipe;

namespace MF.Engineering.MF8910.GestureDetector.DataSources
{
    public class Person
    {
        private bool active;
        private SortedDictionary<long, SmothendSkeleton> skeletons;
        private Device dev;
        private int id;

        public Person(Device d)
        {
            Random r = new Random();
            skeletons = new SortedDictionary<long, SmothendSkeleton>(new DescendingTimeComparer<long>());
            dev = d;
            id = r.Next();
            /*
            WaveGestureChecker wave = new WaveGestureChecker(this);
            wave.Successful += OnWave;
            wave.Failed += delegate(object o, EventArgs e) { Console.WriteLine("fail"); };
            */
            //ZoomGestureChecker zoom = new ZoomGestureChecker(this);
            //zoom.Successful += delegate(object o, GestureEventArgs ev)
            //{
            //    this.OnZoom(this, ev);
            //};
            //zoom.Failed += delegate(object o, GestureEventArgs e) 
            //{ 
            //    Console.WriteLine("zoom fail"); 
            //};
            SwipeGestureChecker swipe = new SwipeGestureChecker(this);
            swipe.Successful += delegate(object o, GestureEventArgs e)
            {
                Console.WriteLine("SWIPED: " + ((SwipeGestureEventArgs)e).Direction.ToString());
            };
            swipe.Failed += delegate(object o, FailedGestureEventArgs e)
            {
                Console.WriteLine("FAIL: " + ((FailedGestureEventArgs)e).Condition.GetType().Name);
            };
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
            long t = System.DateTime.Now.Ticks;
            if (!skeletons.ContainsKey(t))
            {
                skeletons.Add(t, ss);
            }
            else
            {
                skeletons[t] = ss;
            }

            if (skeletons.Count > 10)
            {
                skeletons.Remove(skeletons.ElementAt(9).Key);
            }

            if (NewSkeleton != null)
            {
                NewSkeleton(this, new NewSkeletonEventArgs(ss));
            }
        }

        public SmothendSkeleton CurrentSkeleton
        {
            get
            {
                if (skeletons.Count == 0)
                {
                    return new SmothendSkeleton(new Microsoft.Kinect.Skeleton());
                }
                return skeletons.First().Value;
            }
        }

        public SmothendSkeleton GetLastSkeleton(int i)
        {
            if (i > skeletons.Count-1)
            {
                return null;
            }
            return skeletons.ElementAt(i).Value;
        }

        public long MillisBetweenFrames(int first, int second)
        {
            long diff = (skeletons.ElementAt(second).Key - skeletons.ElementAt(first).Key) / 10;
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

        public double Match(SmothendSkeleton skeleton)
        {
            SkeletonPoint currentRoot = this.CurrentSkeleton.GetPosition(JointType.HipCenter);
            SkeletonPoint otherRoot = skeleton.GetPosition(JointType.HipCenter);
            return SkeletonMath.DistanceBetweenPoints(currentRoot, otherRoot);
        }

        public event EventHandler<NewSkeletonEventArgs> NewSkeleton;
        public event EventHandler<PersonPassiveEventArgs> PersonPassive;
        public event EventHandler<ActivePersonEventArgs> PersonActive;
        public event EventHandler<PersonDisposedEventArgs> PersonDisposed;

        public event EventHandler<GestureEventArgs> OnWave;
        public event EventHandler<GestureEventArgs> OnZoom;
        public event EventHandler<PersonDisposedEventArgs> OnDispose;
        //public event EventHandler blablub;
        //public event EventHandler etc;

        private class DescendingTimeComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T first, T second)
            {
                if (((long)((object)first)) < 1000 &&  ((long)((object)second)) > (long.MaxValue - 1000))
                {
                    return 1;
                }
                if (((long)((object)first)) < ((long)((object)second)))
                {
                    return 1;
                }
                if (((long)((object)first)) > ((long)((object)second)))
                {
                    return -1;
                }
                return 0;
            }
        }

        internal void prepareToDie()
        {
            OnWave = null;
            OnZoom = null;
            OnDispose(this, new PersonDisposedEventArgs(this));
        }
    }
}
