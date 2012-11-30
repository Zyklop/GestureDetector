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
        private const double LOWER_BOUND_FOR_VELOCITY = 3;
        private int index = 0;

        public SwipeCondition(Person p, JointType leftOrRightHand)
            : base(p)
        {
            hand = leftOrRightHand;
            checker = new Checker(p);
        }

        protected override void check(object sender, NewSkeletonEventArgs e)
        {
            List<Direction> handToHipOrientation = checker.GetRelativePosition(JointType.HipCenter, hand);
            List<Direction> handToShoulderOrientation = checker.GetRelativePosition(JointType.ShoulderCenter, hand);
            List<Direction> handMovement = checker.GetAbsoluteMovement(hand);
            double handVelocity = checker.GetAbsoluteVelocity(hand);
            //min speed is maintained
            if (handVelocity < LOWER_BOUND_FOR_VELOCITY)
            {
                Reset();
            }
                // hand is in front of the body and between hip and shoulders
            else if (handToHipOrientation.Contains(Direction.forward)
                && handToHipOrientation.Contains(Direction.upward)
                && handToShoulderOrientation.Contains(Direction.downward))
            {
                // movement did not start yet, initializing
                if (direction == Direction.none)
                {
                    // left or right movement is prefered
                    if (handMovement.Contains(Direction.left))
                    {
                        direction = Direction.left;
                    }
                    else if (handMovement.Contains(Direction.right))
                    {
                        direction = Direction.right;
                    }
                    else
                    {
                        // take other direction
                        direction = handMovement.FirstOrDefault();
                    }
                }
                else if (!handMovement.Contains(direction))
                {
                    // direction changed
                    Reset();
                }
                else
                {
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
                        // step successful, waiting for next
                        index++;
                    }
                }
            }
        }
        // restart detecting
        private void Reset()
        {
            index = 0;
            direction = Direction.none;
            fireFailed(this, new FailedGestureEventArgs()
            {
                Condition = this
            });
        }
    }
}
