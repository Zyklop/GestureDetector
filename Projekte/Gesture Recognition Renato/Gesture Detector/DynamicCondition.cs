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

        protected abstract void check(object src, NewSkeletonEventArg e);

        public event EventHandler Succeded;
        public event EventHandler Failed;
    }
}
