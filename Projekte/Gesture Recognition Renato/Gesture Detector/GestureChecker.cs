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
    /// <summary>
    /// Base class for gesture recognition. It's a state machine which knows 
    /// in which state a gesture currently is. 
    /// - If a gesture part (Condition) was successful, it moves to the next part. 
    /// - If a gesture part failed, the recognition is reset.
    /// - If all gesture parts were successful, this class calls all registered success handlers.</summary>
    class GestureChecker
    {
        /// <summary>
        /// List of gesture parts</summary>
        private List<Condition> conditions;

        /// <summary>
        /// Gesture state: Points to current gesture part (Condition)</summary>
        private IEnumerator<Condition> index;

        /// <summary>
        /// How long a full gesture can take in maximum</summary>
        private long timeout;

        /// <summary>
        /// Time keeper: Points to the time when the gesture (re)started</summary>
        private long startTime;

        /// <summary>
        /// Taking a list of conditions, which are gesture parts to be checked in order
        /// and a timeout indicating how long a full gesture can take in maximum.
        /// </summary>
        /// <param name="gestureConditions">
        /// List of condition which are to fullfill for a successful gesture</param>
        /// <param name="timeout">
        /// Maximum time a gesture is allowed to run for.</param>
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
            this.index.MoveNext();
            this.reset();
        }

        /// <summary>
        /// Reset state machine. Includes timeouts and condition list.</summary>
        public void reset()
        {
            startTime = CurrentMillis.Millis;
            /**
             * Disable all conditions although there should be only one enabled: the last on index.Current
             * But since it can be NULL and there could occurr Exceptions in user code,
             * we invest a bit performance to securely save gesture checking performance.
             */
            foreach (Condition c in conditions)
            {
                c.disable();
            }
            index.Reset();
            index.MoveNext();
            index.Current.enable();
        }

        #region Events

        /// <summary>
        /// Called when a gesture was recognized. That means that all gesture 
        /// parts were sucessfully recognized.</summary>
        public virtual event EventHandler<GestureEventArgs> Successful;

        /// <summary>
        /// Called when at least one gesture part failed.</summary>
        public virtual event EventHandler<FailedGestureEventArgs> Failed;

        /// <summary>
        /// Every time when a condition is checked, we check if its in time.</summary>
        /// <param name="src">
        /// The checked Condition</param>
        /// <param name="e">
        /// Probably empty</param>
        private void ConditionChecked(Object src, EventArgs e)
        {
            if (startTime <= CurrentMillis.Millis - timeout)
            {
                ConditionFailed(this, new FailedGestureEventArgs() {
                    Condition = (Condition)src
                });
            }
        }

        /// <summary>
        /// A gesture part failed. Lets start from the beginning.</summary>
        /// <param name="src">
        /// The checked Condition</param>
        /// <param name="e">
        /// Details about the fail</param>
        private void ConditionFailed(Object src, FailedGestureEventArgs e)
        {
            this.reset();
            if (Failed != null) 
            {
                fireFailed(this, e);
            }
        }

        /// <summary>
        /// Current gesture part was sucessful. Continue with next.</summary>
        /// <param name="src">
        /// The checked condition</param>
        /// <param name="e">
        /// Details about the success</param>
        private void ConditionComplete(Object src, GestureEventArgs e)
        {
            Condition previous = index.Current;
            Boolean hasNext = index.MoveNext();
            Condition next = index.Current;

            if (hasNext) // no further gesture parts -> success!
            {
                previous.disable();
                next.enable();
            }
            else
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
