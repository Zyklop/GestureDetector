using System.Collections.Generic;
using System.Linq;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Tools;
using MF.Engineering.MF8910.GestureDetector.Events;
using Microsoft.Kinect;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Swipe
{
    /// <summary>
    /// Swipe Condition
    /// Checks if  left or right hand moves fast to the left or the right.
    /// </summary>
    class SwipeCondition: DynamicCondition
    {
        protected JointType Hand;
        protected Direction Direction;

        private Checker Checker;
        private const int LowerBoundForSuccess = 2;
        private const double LowerBoundForVelocity = 2.5;
        private int _index;

        public SwipeCondition(Person p, JointType leftOrRightHand)
            : base(p)
        {
            _index = 0;
            Hand = leftOrRightHand;
            Checker = new Checker(p);
        }

        protected override void Check(object sender, NewSkeletonEventArgs e)
        {
            List<Direction> handToHipOrientation = Checker.GetRelativePosition(JointType.HipCenter, Hand).ToList();
            List<Direction> handToShoulderOrientation = Checker.GetRelativePosition(JointType.ShoulderCenter, Hand).ToList();
            List<Direction> handMovement = Checker.GetAbsoluteMovement(Hand).ToList();
            double handVelocity = Checker.GetRelativeVelocity(JointType.HipCenter,Hand);
            //Debug.WriteLine(handVelocity);
            //min speed is maintained
            if (handVelocity < LowerBoundForVelocity)
            {
                Reset();
            }
                // hand is in front of the body and between hip and shoulders
            else if (handToHipOrientation.Contains(Direction.Forward)
                && handToHipOrientation.Contains(Direction.Upward)
                && handToShoulderOrientation.Contains(Direction.Downward))
            {
                // movement did not start yet, initializing
                if (Direction == Direction.None)
                {
                    // left or right movement is prefered
                    if (handMovement.Contains(Direction.Left))
                    {
                        Direction = Direction.Left;
                    }
                    else if (handMovement.Contains(Direction.Right))
                    {
                        Direction = Direction.Right;
                    }
                    else
                    {
                        // take other direction
                        //direction = handMovement.FirstOrDefault();
                    }
                }
                else if (!handMovement.Contains(Direction))
                {
                    // direction changed
                    Reset();
                }
                else
                {
                    if (_index >= LowerBoundForSuccess)
                    {
                        _index = 0;
                        FireSucceeded(this, new SwipeGestureEventArgs
                            {
                            Direction = Direction
                        });
                        Direction = Direction.None;
                    }
                    else
                    {
                        // step successful, waiting for next
                        _index++;
                    }
                }
            }
        }
        // restart detecting
        private void Reset()
        {
            _index = 0;
            Direction = Direction.None;
            FireFailed(this, new FailedGestureEventArgs
                {
                Condition = this
            });
        }
    }
}
