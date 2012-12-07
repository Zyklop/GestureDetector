﻿using System;
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
        private Queue <SmothendSkeleton> skeletons;
        private Device dev;
        private WaveGestureChecker wave;
        private ZoomGestureChecker zoom;
        private SwipeGestureChecker swipe;
        private int id;

        public Person(Device d)
        {
            Random r = new Random();
            skeletons = new Queue < SmothendSkeleton>(); // newest skeletons are first
            dev = d;
            id = r.Next();
            wave = new WaveGestureChecker(this);
            wave.Successful += Waving;
            /*
            wave.Failed += delegate(object o, EventArgs e) { Console.WriteLine("fail"); };
            */
            zoom = new ZoomGestureChecker(this);
            zoom.Successful += delegate(object o, GestureEventArgs ev)
            {
                if (OnZoom != null)
                {
                    this.OnZoom(this, ev);
                }
            };
            //zoom.Failed += delegate(object o, GestureEventArgs e) 
            //{ 
            //    Console.WriteLine("zoom fail"); 
            //};
            swipe = new SwipeGestureChecker(this);
            swipe.Successful += delegate(object o, GestureEventArgs e)
            {
                if (OnSwipe != null)
                {
                    OnSwipe(this, e);
                }
                //Console.WriteLine("SWIPED: " + ((SwipeGestureEventArgs)e).Direction.ToString());
            };
            swipe.Failed += delegate(object o, FailedGestureEventArgs e)
            {
                //Console.WriteLine("FAIL: " + ((FailedGestureEventArgs)e).Condition.GetType().Name);
            };
        }

        private void Waving(object sender, GestureEventArgs e)
        {
            //Debug.WriteLine("waved");
            if (OnWave != null)
            {
                OnWave(this, e);
            }
        }

        public void AddSkeleton(SmothendSkeleton ss)
        {
            if (NewSkeleton != null)
            {
                skeletons.Enqueue(ss);
                NewSkeleton(this, new NewSkeletonEventArgs(ss)); // Event for conditions
            }
            if (skeletons.Count >= 10)
            {
                skeletons.Dequeue(); // remove old unneded
            }
        }

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

        public SmothendSkeleton GetLastSkeleton(int i) //get a previous skeleton
        {
            if (i > skeletons.Count-1 || i < 0)
            {
                return null;
            }
            return skeletons.ElementAt(skeletons.Count-i-1);
        }

        public long MillisBetweenFrames(int first, int second) //get timedifference in millisconds between skeletons
        {
            long diff = (GetLastSkeleton(second).Timestamp - GetLastSkeleton(first).Timestamp);
            //Debug.WriteLineIf(diff < 0, "Time Difference negative in MillisBetweenFrame");
            return diff;
        }

        public bool Active {
            get
            {
                return active;
            }
            set
            {
                active = value;
                if (PersonActive != null && active == true)
                {
                    PersonActive(this, new ActivePersonEventArgs(this));
                }
                else if (PersonPassive != null)
                {
                    PersonPassive(this, new PersonPassiveEventArgs(this));
                }
            }}

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

        internal double Match(SmothendSkeleton skeleton) // distance to other person
        {
            SkeletonPoint currentRoot = this.CurrentSkeleton.GetPosition(JointType.HipCenter);
            SkeletonPoint otherRoot = skeleton.GetPosition(JointType.HipCenter);
            return SkeletonMath.DistanceBetweenPoints(currentRoot, otherRoot);
        }

        public event EventHandler<NewSkeletonEventArgs> NewSkeleton;
        public event EventHandler<PersonPassiveEventArgs> PersonPassive;
        public event EventHandler<ActivePersonEventArgs> PersonActive;

        public event EventHandler<GestureEventArgs> OnWave;
        public event EventHandler<GestureEventArgs> OnZoom;
        public event EventHandler<GestureEventArgs> OnSwipe;
    }
}
