using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gesture_Detection_Api
{
    interface ICondition
    {
        public abstract bool check();

        public event EventHandler Succeded;

        public event EventHandler Failed;
    }
}
