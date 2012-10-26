﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;
using GestureEvents;

namespace Conditions
{
    class StaticCondition:ICondition
    {
        protected Person person;
        private int StartTime;

        public StaticCondition(Person p)
        {
            person = p;
            person.NewSkeleton += check;
        }

        public event EventHandler Triggered;

        //protected virtual void fireSucceded(object src, EventArgs e)
        //{
        //    Succeded(src, e);
        //}

        public event EventHandler Recogniced;

        //protected virtual void fireFailed(object src, EventArgs e)
        //{
        //    Failed(src, e);
        //}

        virtual void check(object src, NewSkeletonEventArg e)
        {
            // check
        }
    }
}