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
    /**
     * Base class for gesture recognition. It's a state machine which knows 
     * in which state a gesture currently is. 
     * - If a gesture part (Condition) was successful, it moves to the next part. 
     * - If a gesture part failed, the recognition is reset.
     * - If all gesture parts were successful, this class calls all registered success handlers.
     */
    class GestureChecker
    {
        /**
         * List of gesture parts
         */
        private List<Condition> conditions;

        /**
         * Gesture state: Points to current gesture
         */
        private IEnumerator<Condition> index;

        /**
         * How long a full gesture can take in maximum
         */
        private long timeout;

        /**
         * Time keeper
         */
        private long startTime;

        /**
         * Taking a list of conditions, which are gesture parts to be checked in order
         * and a timeout indicating how long a full gesture can take in maximum.
         */
        public GestureChecker(List<Condition> gestureConditions, int timeout)
        {
            this.timeout = timeout;

            conditions = gestureConditions;
            conditions.ForEach(delegate(Condition c) {
                c.OnCheck += ConditionChecked;
                c.Succeeded += ConditionComplete;
                c.Failed += ConditionFailed;
            });

            this.index = conditions.GetEnumerator();
            this.reset();
        }

        /**
         * Reset state machine. Includes timeouts and condition list.
         */
        public void reset()
        {
            startTime = CurrentMillis.Millis;
            if (index.Current != null) 
            {
                index.Current.disable();
            }
            index.Reset();
            index.MoveNext();
            index.Current.enable();
        }

        #region Events

        public virtual event EventHandler<GestureEventArgs> Successful;
        public virtual event EventHandler<FailedGestureEventArgs> Failed;

        /**
         * Every time when a condition is checked, we check if its in time
         */
        private void ConditionChecked(Object src, EventArgs e)
        {
            if (startTime <= CurrentMillis.Millis - timeout)
            {
                ConditionFailed(this, new FailedGestureEventArgs() {
                    Condition = (Condition)src
                });
            }
        }

        /**
         * A gesture part failed. Lets start from the beginning.
         */
        private void ConditionFailed(Object src, FailedGestureEventArgs e)
        {
            this.reset();
            if (Failed != null) 
            {
                fireFailed(this, e);
            }
        }

        /**
         * Gesture part is complete. Continue with next.
         */
        private void ConditionComplete(Object src, GestureEventArgs e)
        {
            Condition previous = index.Current;
            Boolean hasNext = index.MoveNext();

            if (hasNext)
            { // there are further gesture parts
                previous.disable();
                index.Current.enable(); 
            }
            else // no further gesture parts -> success!
            {
                this.reset();
                fireSucessful(this, e);
            }
        }

        protected virtual void fireSucessful(Object sender, GestureEventArgs e)
        {
            if (Successful != null)
            {
                Successful(this, e);
            }
        }


        protected virtual void fireFailed(Object sender, FailedGestureEventArgs e)
        {
            if (Failed != null)
            {
                Failed(this, e);
            }
        }

        #endregion
    }
}
