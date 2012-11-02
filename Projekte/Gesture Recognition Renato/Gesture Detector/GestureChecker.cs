using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Conditions;
using DataSources;
using System.Timers;
using System.Collections;

namespace Gesture_Detector
{
    class GestureChecker
    {
        private List<Condition> conditions;
        private IEnumerator index;
        private Timer timer;

        // timeout in ms
        public GestureChecker(List<Condition> gestureConditions, int timeout)
        {
            conditions = gestureConditions;
            conditions.ForEach(delegate(Condition c) { 
                c.Succeeded += ConditionComplete;
            });

            index = conditions.GetEnumerator();
            index.MoveNext();

            timer = new Timer(timeout);
            timer.Start();
            timer.Elapsed += Timeout;
        }

        #region Events

        public event EventHandler<EventArgs> Successful;
        public event EventHandler<EventArgs> Failed;

        private void ConditionFailed(Object src, EventArgs e)
        {
            timer.Stop();
            if (Failed != null) 
            {
                Failed(this, new EventArgs());
            }
            index.Reset();
            timer.Start();
        }

        private void ConditionComplete(Object src, EventArgs e)
        {
            if (!index.MoveNext())
            {
                Console.WriteLine("GACHGACH - PSSST RENATO!");
                if (Successful != null) {
                    Successful(this, new EventArgs());
                }
                index.Reset();
            }
        }

        private void Timeout(Object src, EventArgs e)
        {
            timer.Stop();
            Failed(this, new EventArgs());
            index.Reset();
            timer.Start();
        }

        #endregion
    }
}
