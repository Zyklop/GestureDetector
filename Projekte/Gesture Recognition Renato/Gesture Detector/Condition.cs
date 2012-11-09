﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;

namespace MF.Engineering.MF8910.GestureDetector.Gestures
{
    public abstract class Condition
    {
        protected Person person;

        public Condition(Person p) 
        { 
            person = p;
        }

        public void enable()
        {
            person.NewSkeleton += check;
        }

        public void disable()
        {
            person.NewSkeleton -= check;
        }

        protected abstract void check(object src, NewSkeletonEventArgs e);

        #region Events

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
