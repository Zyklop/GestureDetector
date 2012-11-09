using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Timers;
using System.Collections;
using System.Diagnostics;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Gestures
{
    class GestureChecker
    {
        private List<Condition> conditions;
        private IEnumerator<Condition> index;
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
            index.Current.enable(); // beginne ersten Gestenteil zu checken

            timer = new Timer(timeout);
            timer.Start();
            timer.Elapsed += Timeout;
        }

        #region Events

        public event EventHandler<EventArgs> Successful;
        public event EventHandler<EventArgs> Failed;

        private void ConditionFailed(Object src, EventArgs e)
        {
            Debug.WriteLine(index.Current.GetType().Name + " failed.");
            timer.Stop();
            if (Failed != null) 
            {
                Failed(this, new EventArgs());
            }
            index.Reset();
            index.MoveNext();
            timer.Start();
        }

        /**
         * Gestenteil ist komplett. Fahre mit dem nächsten weiter.
         */
        private void ConditionComplete(Object src, EventArgs e)
        {
            Debug.WriteLine(index.Current.GetType().Name + " complete.");
            index.Current.disable(); // checke vollendeten Gestenteil nicht mehr
            Boolean hasNext = index.MoveNext();
            if (!hasNext) // keine weiteren Gestenteile vorhanden -> Erfolg
            {
                Debug.WriteLine("Success!");
                if (Successful != null)
                {
                    Successful(this, new EventArgs());
                }
                index.Reset();
                index.MoveNext();
            }
            index.Current.enable(); // checke den nächsten Gestenteil
        }

        private void Timeout(Object src, EventArgs e)
        {
            Debug.WriteLine("timed out.");
            timer.Stop();
            Failed(this, new EventArgs());
            timer.Start();
        }

        #endregion
    }
}
