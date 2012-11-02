using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;
using GestureEvents;

namespace Conditions
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

        public event EventHandler<EventArgs> Succeeded;

        public event EventHandler<EventArgs> Failed;

        protected void fireSucceeded(object sender, EventArgs e)
        {
            Succeeded(sender, e);
        }

        protected void fireFailed(object sender, EventArgs e)
        {
            Failed(sender, e);
        }

        #endregion
    }
}
