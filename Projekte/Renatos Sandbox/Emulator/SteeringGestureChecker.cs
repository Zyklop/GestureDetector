using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures;
using MF.Engineering.MF8910.GestureDetector.Tools;
using Microsoft.Kinect;


namespace Emulator
{
    class SteeringGestureChecker:GestureChecker
    {
        protected const int ConditionTimeout = 2500;

        public SteeringGestureChecker(Person p)
            : base(new List<Condition> {

                new SteeringCondition(p)

            }, ConditionTimeout)
        {
        }
    }

    internal class SteeringCondition : Condition
    {
        private Checker checker;

        public SteeringCondition(Person person) : base(person)
        {
            checker = new Checker(person);
        }

        protected override void Check(object src, NewSkeletonEventArgs e)
        {
            List<Direction> leftToRightHand =
                checker.GetRelativePosition(JointType.HandLeft, JointType.HandRight).ToList();
            if (leftToRightHand.Contains(Direction.Upward))
            {
                FireSucceeded(this, new SteeringGestureEventArgs{Direction = Direction.Left});
            }
            else if (leftToRightHand.Contains(Direction.Downward))
            {
                FireSucceeded(this, new SteeringGestureEventArgs { Direction = Direction.Right });
            }
            else
            {
                FireSucceeded(this, new SteeringGestureEventArgs { Direction = Direction.None });
            }
        }
    }
}
