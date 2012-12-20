using System.Collections.Generic;
using Microsoft.Kinect;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Swipe
{
    class SwipeGestureChecker: GestureChecker
    {
        protected const int ConditionTimeout = 1500;

        public SwipeGestureChecker(Person p)
            : base(new List<Condition> {

                new SwipeCondition(p, JointType.HandRight)

            }, ConditionTimeout) { }
    }
}
