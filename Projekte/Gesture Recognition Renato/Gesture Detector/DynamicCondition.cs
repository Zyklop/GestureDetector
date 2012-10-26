using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;
using GestureEvents;

namespace Conditions
{
    abstract class DynamicCondition: ICondition
    {
        protected Person person;

        public DynamicCondition(Person p) 
        { 
            person = p;
            person.NewSkeleton += check;
        }

        public event EventHandler Succeded;

        //protected virtual void fireSucceded(object src, EventArgs e)
        //{
        //    Succeded(src, e);
        //}

        public event EventHandler Failed;

        //protected virtual void fireFailed(object src, EventArgs e)
        //{
        //    Failed(src, e);
        //}

        protected abstract void check(object src, NewSkeletonEventArg e);
    }
}
