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
        private List<StaticCondition> Always;
        private List<ICondition> Consequtive;
        private List<ICondition> Once;
        private int index=0;
        private List<bool> onceRes;
        private Timer tim;

        public GestureChecker (List<StaticCondition> always, List<ICondition> consequtive, List<ICondition> once, int time)
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
                Always = new List<StaticCondition>();
            }
            if (Consequtive == null)
            {
                Consequtive = new List<ICondition>();
            }
            if (Once==null)
            {
                Once = new List<ICondition>();
            }
            foreach (StaticCondition cond in Always)
            {
                cond.Failed += awFailed;
            }
            foreach (ICondition cond in Once)
            {
                if (cond is StaticCondition)
                {
                    ((StaticCondition)cond).Succeded += onceOk;
                }
                else
                {
                    ((DynamicCondition)cond).Triggered += onceOk;
                }
                onceRes.Add(false);
            }
            foreach (ICondition cond in Consequtive)
            {
                if (cond is StaticCondition)
                {
                    ((StaticCondition)cond).Succeded += conseqOk;
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
            if (Consequtive.IndexOf((ICondition)src) == index)
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
            onceRes[Once.IndexOf((ICondition)src)] = true;
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
