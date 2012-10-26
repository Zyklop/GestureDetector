using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;
using GestureEvents;

namespace Conditions
{
    abstract class StaticCondition: ICondition
    {
        protected Person person;
        private int StartTime;

        public StaticCondition(Person p)
        {
            person = p;
            person.NewSkeleton += check;
        }

        protected abstract void check(object src, NewSkeletonEventArg e);

        public event EventHandler Triggered;
        public event EventHandler Recogniced;
    }
}
