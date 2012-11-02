using Conditions;
using Conditions.Login;
using DataSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesture_Detector
{
    class WaveGestureChecker: GestureChecker
    {
        protected const int CONDITION_TIMEOUT = 2500;

        public WaveGestureChecker(Person p)
            : base(new List<Condition> {

                new WaveLeftCondition(p),
                new WaveRightCondition(p)

            }, CONDITION_TIMEOUT) { }
    }
}
