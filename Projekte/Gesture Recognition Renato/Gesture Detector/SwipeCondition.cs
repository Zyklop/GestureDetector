using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Tools;
using MF.Engineering.MF8910.GestureDetector.Events;
using Microsoft.Kinect;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Swipe
{
    class SwipeCondition: DynamicCondition
    {
        protected JointType hand;
        protected Direction direction;

        private Checker checker;
        private const int LOWER_BOUND_FOR_SUCCESS = 2;
        private const double LOWER_BOUND_FOR_VELOCITY = 0.005;
        private int index = 0;

        public SwipeCondition(Person p, JointType leftOrRightHand, Direction d)
            : base(p)
        {
            hand = leftOrRightHand;
            direction = d;
            checker = new Checker(p);
        }

        protected override void check(object sender, NewSkeletonEventArgs e)
        {
            List<Direction> handOrientation = checker.GetRelativePosition(JointType.HipCenter, hand);
            List<Direction> handMovement = checker.GetAbsoluteMovement(hand);
            double handVelocity = checker.GetAbsoluteVelocity(hand);

            if (handVelocity > LOWER_BOUND_FOR_VELOCITY)
            {
                Console.WriteLine("V: "+handVelocity);
            }

            if (handOrientation.Contains(Direction.forward)
                && handMovement.Contains(direction)
                && handVelocity >= LOWER_BOUND_FOR_VELOCITY)
            {
                Console.WriteLine("++");
                if (index >= LOWER_BOUND_FOR_SUCCESS)
                {
                    index = 0;
                    fireSucceeded(this, new SwipeGestureEventArgs()
                    {
                        Direction = direction
                    });
                }
                else
                {
                    index++;
                }
            }
            else
            {
                fireFailed(this, new FailedGestureEventArgs()
                {
                    Condition = this
                });
            }
        }
    }
}
