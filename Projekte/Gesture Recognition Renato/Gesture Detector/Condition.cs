using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;

namespace MF.Engineering.MF8910.GestureDetector.Gestures
{
    /**
     * Base class for static gesture parts. They just succeed or fail.
     */
    public abstract class Condition
    {
        protected Person person;

        /**
         * Create a gesture part, whose fullfillment is checked on Person p.
         */
        public Condition(Person p) 
        { 
            person = p;
        }

        /**
         * Begin checking new skeletons.
         * Save performance and enable only gestures you really need to check.
         */
        public void enable()
        {
            person.NewSkeleton += extendedCheck;
        }

        /**
         * Dont react on new skeletons (anymore).
         * Use this to save performance after a gesture isn't used anymore.
         */
        public void disable()
        {
            person.NewSkeleton -= extendedCheck;
        }

        /**
         * Implement this to check for a gesture part.
         */
        protected abstract void check(object src, NewSkeletonEventArgs e);

        /**
         * Since it's up to the user to override "check" correctly, there's the possibility that
         * he never triggered an event to make the GestureChecker proceed. Therefore we publish
         * that we performed a "check" and consumed time in the GestureChecked state machine.
         */
        private void extendedCheck(object src, NewSkeletonEventArgs args)
        {
            check(src, args);
            OnCheck(this, new EventArgs());
        }

        #region Events

        public event EventHandler<EventArgs> OnCheck;
        public event EventHandler<GestureEventArgs> Succeeded;
        public event EventHandler<FailedGestureEventArgs> Failed;

        protected void fireSucceeded(object sender, GestureEventArgs e)
        {
            if (Succeeded != null)
            {
                Succeeded(sender, e);
            }
        }

        protected void fireFailed(object sender, FailedGestureEventArgs e)
        {
            if (Failed != null)
            {
                Failed(sender, e);
            }
        }

        #endregion
    }
}
