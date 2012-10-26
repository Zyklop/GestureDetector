using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Conditions;
using DataSources;
using System.Timers;

namespace Gesture_Detector
{
    class GestureChecker
    {
        private List<Condition> Always;
        private List<Condition> Consequtive;
        private List<Condition> Once;
        private int index=0;
        private List<bool> onceRes;
        private Timer tim;

        public GestureChecker (List<Condition> always, List<Condition> consequtive, List<Condition> once, int time)
        {
            Always = always;
            Consequtive = consequtive;
            Once = once;
            tim = new Timer(time);
            tim.Start();
            tim.Elapsed += timeout;
            onceRes = new List<bool>();
            if (Always == null)
            {
                Always = new List<Condition>();
            }
            if (Consequtive == null)
            {
                Consequtive = new List<Condition>();
            }
            if (Once==null)
            {
                Once = new List<Condition>();
            }
            foreach (Condition cond in Always)
            {
                cond.Failed += awFailed;
            }
            foreach (Condition cond in Once)
            {
                if (cond is Condition)
                {
                    cond.Succeeded += onceOk;
                }
                else
                {
                    ((DynamicCondition)cond).Triggered += onceOk;
                }
                onceRes.Add(false);
            }
            foreach (Condition cond in Consequtive)
            {
                if (cond is Condition)
                {
                    cond.Succeeded += conseqOk;
                }
                else
                {
                    ((DynamicCondition)cond).Triggered += conseqOk;
                }
            }
        }

        void awFailed(Object src, EventArgs e)
        {
            Failed(this, new EventArgs());
            tim.Stop();
            tim.Start();
            resetOnce();
            index = 0;
        }

        private void resetOnce()
        {
            onceRes.ConvertAll(n => false);
        }

        void conseqOk(Object src, EventArgs e)
        {
            if (Consequtive.IndexOf((Condition)src) == index)
            {
                index++;
                checkComplete();
            }
        }

        private void checkComplete()
        {
            if (!onceRes.Contains(false) && index == Consequtive.Count-1)
            {
                Succesfull(this, new EventArgs());
            }
        }

        void onceOk(Object src, EventArgs e)
        {
            onceRes[Once.IndexOf((Condition)src)] = true;
        }

        void timeout(Object src, EventArgs e)
        {
            Failed(this, new EventArgs());
            tim.Stop();
            tim.Start();
            resetOnce();
            index = 0;
        }

        public event EventHandler<EventArgs> Succesfull;

        public event EventHandler<EventArgs> Failed;
    }
}
