using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Tools;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Swipe
{
    class SwipeGestureChecker: GestureChecker
    {
        protected const int CONDITION_TIMEOUT = 1500;

        public SwipeGestureChecker(Person p)
            : base(new List<Condition> {

                new SwipeCondition(p, JointType.HandRight, Direction.left)

            }, CONDITION_TIMEOUT) { }
    }
}
