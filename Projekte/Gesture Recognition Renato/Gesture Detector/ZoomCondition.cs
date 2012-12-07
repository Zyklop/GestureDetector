using MF.Engineering.MF8910.GestureDetector.DataSources;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.Tools;
using MF.Engineering.MF8910.GestureDetector.Events;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Zoom
{
    class ZoomCondition : DynamicCondition
    {
        private const double UPPER_BOUND_FOR_VELOCITY = 1.5;
        private const int LOWER_BOUND_TO_BEGIN = 30;
        private int index = 0;
        // TODO set Distance, positive = zoomOut, neg = zoomIn
        //private int zoomDistance = 0;
        private Checker checker;
        List<Direction> rightHandToHipOrientation, leftHandToHipOrientation, rightHandToHeadOrientation,
            leftHandToHeadOrientation, leftHandToRightHandDirection;
        double rightHandVelocity, leftHandVelocity;

        public ZoomCondition(Person p)
            : base(p)
        {
            checker = new Checker(p);
        }

        protected override void check(object sender, NewSkeletonEventArgs e)
        {
            rightHandToHipOrientation = checker.GetRelativePosition(JointType.HipLeft, JointType.HandRight);
            leftHandToHipOrientation = checker.GetRelativePosition(JointType.HipRight, JointType.HandLeft);
            rightHandToHeadOrientation = checker.GetRelativePosition(JointType.Head, JointType.HandRight);
            leftHandToHeadOrientation = checker.GetRelativePosition(JointType.Head, JointType.HandLeft);
            leftHandToRightHandDirection = checker.GetRelativePosition(JointType.HandRight, JointType.HandLeft);
            rightHandVelocity = checker.GetRelativeVelocity(JointType.HipCenter, JointType.HandRight);
            leftHandVelocity = checker.GetRelativeVelocity(JointType.HipCenter, JointType.HandLeft);

            // Both Hands have to be in front of the body, between head and hip, and on their corresponding side
            // but not together or crossed
            if (rightHandToHipOrientation.Contains(Direction.forward)
                && leftHandToHipOrientation.Contains(Direction.forward)
                && rightHandToHipOrientation.Contains(Direction.upward)
                && leftHandToHipOrientation.Contains(Direction.upward)
                && rightHandToHipOrientation.Contains(Direction.right)
                && leftHandToHipOrientation.Contains(Direction.left)
                && rightHandToHeadOrientation.Contains(Direction.downward)
                && leftHandToHeadOrientation.Contains(Direction.downward)
                && leftHandToRightHandDirection.Contains(Direction.left)
                && (rightHandVelocity <= UPPER_BOUND_FOR_VELOCITY)
                && (leftHandVelocity <= UPPER_BOUND_FOR_VELOCITY))
            {

                if (index >= LOWER_BOUND_TO_BEGIN)
                {
                    fireSucceeded(this, new InternalZoomGestureEventArgs()
                    {
                        Gauge = checker.GetDistanceMedian(JointType.HandRight, JointType.HandLeft)
                    });
                    //Console.WriteLine("Success! Velocity: right " + rightHandVelocity + ", left " + leftHandVelocity);
                }
                else
                {
                    index++;
                    fireTriggered(this, new InternalZoomGestureEventArgs()
                    {
                        Gauge = checker.GetDistanceMedian(JointType.HandRight, JointType.HandLeft)
                    });
                }
            }
            else
            {
                index -= 11;
                if (index < 0)
                {
                    index = 0;
                    fireFailed(this, new FailedGestureEventArgs()
                        {
                            Condition = this
                        });
                }
            }
        }
    }
}
