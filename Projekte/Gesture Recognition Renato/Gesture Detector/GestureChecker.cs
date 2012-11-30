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
        private long startTime = 0;

        /**
         * Taking a list of conditions, which are gesture parts to be checked in order
         * and a timeout indicating how long a full gesture can take in maximum.
         */
        public GestureChecker(List<Condition> gestureConditions, int timeout)
        {
            this.timeout = timeout;
            this.startTime = CurrentMillis.Millis;

            conditions = gestureConditions;
            conditions.ForEach(delegate(Condition c) {
                c.Succeeded += ConditionComplete;
                c.Failed += ConditionFailed;
            });

            index = conditions.GetEnumerator();
            index.MoveNext();
            index.Current.enable(); // Activate gesture recognition for the first gesture part
        }

        #region Events

        public virtual event EventHandler<GestureEventArgs> Successful;
        public virtual event EventHandler<FailedGestureEventArgs> Failed;

        /**
         * A gesture part failed. Lets start from the beginning.
         */
        private void ConditionFailed(Object src, FailedGestureEventArgs e)
        {
            startTime = CurrentMillis.Millis;
            if (Failed != null) 
            {
                fireFailed(this, e);
            }
            index.Reset();
            index.MoveNext();
        }

        /**
         * Gesture part is complete. Continue with next.
         */
        private void ConditionComplete(Object src, GestureEventArgs e)
        {
            if (startTime <= CurrentMillis.Millis - timeout)
            {
                Timeout();
                return;
            }

            /**
             * When moving forward in the gesture part list, we dont need to
             * check for past gesture parts anymore.
             */
            index.Current.disable();

            Boolean hasNext = index.MoveNext();
            if (!hasNext) // no further gesture parts -> success!
            {
                fireSucessful(this, e);
                index.Reset();
                index.MoveNext();
            }

            /**
             * Activate gesture recognition for the next gesture part.
             * This can be the ordered successor or the initial one.
             */
            index.Current.enable();
        }

        /**
         * Gesture or gesture part did take to long.
         */
        private void Timeout()
        {
            startTime = CurrentMillis.Millis;

            index.Reset();
            index.MoveNext();
            if (Failed != null)
            {
                Failed(this, new FailedGestureEventArgs()
                {
                    Condition = index.Current
                });
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
