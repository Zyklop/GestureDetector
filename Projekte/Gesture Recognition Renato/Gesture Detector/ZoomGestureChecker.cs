using Conditions;
using Conditions.Login;
using DataSources;
using Gesture_Detector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conditions.Zoom
{
    class ZoomGestureChecker: GestureChecker
    {
        protected const int CONDITION_TIMEOUT = 2500;

        public ZoomGestureChecker(Person p)
            : base(new List<Condition> {

                new ZoomCondition(p)

            }, CONDITION_TIMEOUT) { }
    }
}
