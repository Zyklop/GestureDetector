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
    public class SteeringGestureChecker:GestureChecker
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
            int dir = 0;
            if (leftToRightHand.Contains(Direction.Upward))
            {
                dir = -1;
            }
            else if (leftToRightHand.Contains(Direction.Downward))
            {
                dir = 1;
            }
            if (!(leftToRightHand.Contains(Direction.Right)))
            {
                dir *= 2;
            }
            FireSucceeded(this, new SteeringGestureEventArgs { Direction = dir });
        }
    }
}
