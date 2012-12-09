using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;

namespace MF.Engineering.MF8910.GestureDetector.Gestures
{
    /// <summary>
    /// Base class for static gesture parts. They just succeed or fail.</summary>
    public abstract class Condition
    {
        /// <summary>
        /// Person who has to fullfill this condition</summary>
        protected Person person;

        /// <summary>
        /// Create a gesture part, whose fullfillment is checked on Person p.</summary>
        /// <param name="p">
        /// Person who has to fullfill this condition</param>
        public Condition(Person p) 
        { 
            person = p;
        }

        /// <summary>
        /// Begin checking new skeletons.
        /// Save performance and enable only gestures you really need to check.</summary>
        public void enable()
        {
            person.NewSkeleton += extendedCheck;
        }

        /// <summary>
        /// Dont react on new skeletons (anymore).
        /// Use this to save performance after a gesture isn't used anymore.</summary>
        public void disable()
        {
            person.NewSkeleton -= extendedCheck;
        }

        /// <summary>
        /// Implement this to check for the fullfillment of a gesture part.
        /// This method is called every time when a person gets a new skeleton.
        /// It is good practice to fire success or fail in the implementation
        /// of this method. For the checking itself you can use information
        /// about the persons skeletons.</summary>
        /// <param name="src">
        /// The object which fired the event. (This is probably the Device class.)</param>
        /// <param name="e">
        /// NewSkeletonEventArgs contains the person which got a new skeleton.</param>
        protected abstract void check(object src, NewSkeletonEventArgs e);

        /// <summary>
        /// Since it's up to the user to override "check" correctly, there's the
        /// possibility that he never triggered an event to make the GestureChecker proceed. 
        /// Therefore we publish that we performed a "check" and consumed time in the 
        /// GestureChecker state machine.</summary>
        /// <param name="src">
        /// The object which fired the event. (This is probably the Device class.)</param>
        /// <param name="args">
        /// NewSkeletonEventArgs contains the person which got a new skeleton.</param>
        private void extendedCheck(object src, NewSkeletonEventArgs args)
        {
            check(src, args);
            OnCheck(this, new EventArgs());
        }

        #region Events

        /// <summary>
        /// Called every time a condition is checked.</summary>
        public event EventHandler<EventArgs> OnCheck;
        /// <summary>
        /// Called every time a condition successfully completed</summary>
        public event EventHandler<GestureEventArgs> Succeeded;
        /// <summary>
        /// Called every time a condition failed</summary>
        public event EventHandler<FailedGestureEventArgs> Failed;

        /// <summary>
        /// Indicate a call to registered Success Eventhandlers</summary>
        /// <param name="sender">
        /// Probably an implementation of the GestureChecker class</param>
        /// <param name="e">
        /// Detailed arguments for a gesture part</param>
        protected void fireSucceeded(object sender, GestureEventArgs e)
        {
            if (Succeeded != null)
            {
                Succeeded(sender, e);
            }
        }

        /// <summary>
        /// Indicate a call to registered Failed Eventhandlers</summary>
        /// <param name="sender">
        /// Probably an implementation of the GestureChecker class</param>
        /// <param name="e">
        /// Detailed arguments for a gesture part</param>
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
