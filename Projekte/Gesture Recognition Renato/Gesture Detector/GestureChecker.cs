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
using MF.Engineering.MF8910.GestureDetector.Events;

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
                c.Failed += ConditionFailed;
            });

            index = conditions.GetEnumerator();
            index.MoveNext();
            index.Current.enable(); // beginne ersten Gestenteil zu checken

            timer = new Timer(timeout);
            timer.Start();
            timer.Elapsed += Timeout;
        }

        #region Events

        public virtual event EventHandler<GestureEventArgs> Successful;
        public virtual event EventHandler<FailedGestureEventArgs> Failed;

        private void ConditionFailed(Object src, FailedGestureEventArgs e)
        {
            Debug.WriteLine(index.Current.GetType().Name + " failed.");
            timer.Stop();
            if (Failed != null) 
            {
                fireFailed(this, e);
            }
            index.Reset();
            index.MoveNext();
            timer.Start();
        }

        protected virtual void fireFailed(Object sender, FailedGestureEventArgs e)
        {
            if (Failed != null)
            {
                Failed(this, e);
            }
        }

        /**
         * Gestenteil ist komplett. Fahre mit dem nächsten weiter.
         */
        private void ConditionComplete(Object src, GestureEventArgs e)
        {
            Debug.WriteLine(index.Current.GetType().Name + "(" + conditions.IndexOf(index.Current) + ") complete.");
            index.Current.disable(); // checke vollendeten Gestenteil nicht mehr
            Boolean hasNext = index.MoveNext();
            if (!hasNext) // keine weiteren Gestenteile vorhanden -> Erfolg
            {
                //Debug.WriteLine("Success!");
                    fireSucessful(this, e);
                index.Reset();
                index.MoveNext();
            }
            index.Current.enable(); // checke den nächsten Gestenteil
        }

        private void Timeout(Object src, EventArgs e)
        {
            //TODO Debug

            Debug.WriteLine("timed out.");
            timer.Stop();
            index.Reset();
            index.MoveNext();
            if (Failed != null)
            {
                Failed(this, new FailedGestureEventArgs()
                {
                    Condition = index.Current
                });
            }
            timer.Start();
        }

        protected virtual void fireSucessful(Object sender, GestureEventArgs e)
        {
            if (Successful != null)
            {
                Successful(this, e);
            }
        }

        #endregion
    }
}
