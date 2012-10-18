using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gesture_Detection_Api
{
    class GestureChecker
    {
        private List<ICondition> conditions;

        public event EventHandler OnCondition;
    }
}
