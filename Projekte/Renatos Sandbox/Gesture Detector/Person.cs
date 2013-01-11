using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Kinect;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Tools;
using MF.Engineering.MF8910.GestureDetector.Gestures.Wave;
using MF.Engineering.MF8910.GestureDetector.Gestures.Zoom;
using MF.Engineering.MF8910.GestureDetector.Gestures.Swipe;

namespace MF.Engineering.MF8910.GestureDetector.DataSources
{
    /// <summary>
    /// Collection of skeletons for which gestures can be recognized.
    /// A person is unique.</summary>
    public class Person
    {
        private bool _active;
        private Queue <SmothendSkeleton> skeletons;
        private Device _dev;
        private WaveGestureChecker wave;
        //private ZoomGestureChecker zoom;
        //private SwipeGestureChecker swipe;
        private const int SkeletonsToStore = 10;

        public Person(Device d)
        {
            Random r = new Random();
            skeletons = new Queue < SmothendSkeleton>(); // newest skeletons are first
            _dev = d;
            Id = r.Next();
            wave = new WaveGestureChecker(this);
            wave.Successful += Waving;
            /*
            wave.Failed += delegate(object o, EventArgs e) { Console.WriteLine("fail"); };
            */
            //zoom = new ZoomGestureChecker(this);
            //zoom.Successful += delegate(object o, GestureEventArgs ev)
            //{
            //    if (OnZoom != null)
            //    {
            //        OnZoom(this, ev);
            //    }
            //};
            //zoom.Failed += delegate(object o, GestureEventArgs e) 
            //{ 
            //    Console.WriteLine("zoom fail"); 
            //};
            //swipe = new SwipeGestureChecker(this);
            //swipe.Successful += delegate(object o, GestureEventArgs e)
            //{
            //    if (OnSwipe != null)
            //    {
            //        OnSwipe(this, e);
            //    }
            //    //Console.WriteLine("SWIPED: " + ((SwipeGestureEventArgs)e).Direction.ToString());
            //};
            //swipe.Failed += delegate
            //    {
            //    //Console.WriteLine("FAIL: " + ((FailedGestureEventArgs)e).Condition.GetType().Name);
            //};
        }

        private void Waving(object sender, GestureEventArgs e)
        {
            //Debug.WriteLine("waved");
            if (OnWave != null)
            {
                OnWave(this, e);
            }
        }

        /// <summary>
        /// Store a new skeleton
        /// </summary>
        /// <param name="ss"></param>
        internal void AddSkeleton(SmothendSkeleton ss)
        {
            if (NewSkeleton != null)
            {
                skeletons.Enqueue(ss);
                NewSkeleton(this, new NewSkeletonEventArgs(ss)); // Event for conditions
            }
            if (skeletons.Count >= SkeletonsToStore)
            {
                skeletons.Dequeue(); // remove old unneded
            }
        }

        /// <summary>
        /// Get the current skeleton
        /// </summary>
        public SmothendSkeleton CurrentSkeleton
        {
            get
            {
                if (skeletons.Count == 0) // no skeleton avaliable yet
                {
                    return null;
                }
                return skeletons.Last();
            }
        }

        /// <summary>
        /// Get a previous skeleton
        /// </summary>
        /// <param name="i">number of frames back</param>
        /// <returns></returns>
        public SmothendSkeleton GetLastSkeleton(int i) //get a previous skeleton
        {
            if (i > skeletons.Count-1 || i < 0)
            {
                return null;
            }
            return skeletons.ElementAt(skeletons.Count-i-1);
        }

        /// <summary>
        /// Time-difference between two skeletons
        /// </summary>
        /// <param name="first">Relative number of the first frame</param>
        /// <param name="second">Relative number of the second frame</param>
        /// <returns>Milliseconds passed between</returns>
        public long MillisBetweenFrames(int first, int second) //get timedifference in millisconds between skeletons
        {
            long diff = (GetLastSkeleton(second).Timestamp - GetLastSkeleton(first).Timestamp);
            //Debug.WriteLineIf(diff < 0, "Time Difference negative in MillisBetweenFrame");
            return diff;
        }

        /// <summary>
        /// Is the person active?
        /// Firing events when changing
        /// </summary>
        public bool Active {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                if (PersonActive != null && _active)
                {
                    PersonActive(this, new ActivePersonEventArgs(this));
                }
                else if (PersonPassive != null)
                {
                    PersonPassive(this, new PersonPassiveEventArgs(this));
                }
            }}

        /// <summary>
        /// A random id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Should events be triggered when the person is passive?
        /// </summary>
        public bool SendEventsWhenPassive { get; set; }

        public override bool Equals(object p)
        {
            return GetHashCode().Equals(p.GetHashCode());
        }

        public override int GetHashCode()
        {
            return Id;
        }

        internal double Match(SmothendSkeleton skeleton) // distance to other person
        {
            SkeletonPoint currentRoot = CurrentSkeleton.GetPosition(JointType.HipCenter);
            SkeletonPoint otherRoot = skeleton.GetPosition(JointType.HipCenter);
            return SkeletonMath.DistanceBetweenPoints(currentRoot, otherRoot);
        }

        internal event EventHandler<NewSkeletonEventArgs> NewSkeleton;
        public event EventHandler<PersonPassiveEventArgs> PersonPassive;
        public event EventHandler<ActivePersonEventArgs> PersonActive;

        public event EventHandler<GestureEventArgs> OnWave;
        public event EventHandler<GestureEventArgs> OnZoom;
        public event EventHandler<GestureEventArgs> OnSwipe;
    }
}
