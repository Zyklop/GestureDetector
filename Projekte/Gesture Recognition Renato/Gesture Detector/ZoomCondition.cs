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
    class ZoomCondition: Condition
    {
        private const double UPPER_BOUND_FOR_VELOCITY = 0.003;
        private const int LOWER_BOUND_TO_BEGIN = 70;
        private int index = 0;
        // TODO set Distance, positive = zoomOut, neg = zoomIn
        private int zoomDistance = 0;
        private Checker checker;
        List<Direction> rightHandOrientation, leftHandOrientation, rightHandMovement, leftHandMovement;
        double rightHandVelocity, leftHandVelocity;

        public ZoomCondition(Person p)
            : base(p)
        {
            checker = new Checker(p);
        }

        protected override void check(object sender, NewSkeletonEventArgs e)
        {
            rightHandOrientation = checker.GetRelativePosition(JointType.ElbowRight, JointType.HandRight);
            leftHandOrientation = checker.GetRelativePosition(JointType.ElbowLeft, JointType.HandLeft);
            rightHandMovement = checker.GetAbsoluteMovement(JointType.HandRight);
            leftHandMovement = checker.GetAbsoluteMovement(JointType.HandLeft);
            rightHandVelocity = checker.GetAbsoluteVelocity(JointType.HandRight);
            leftHandVelocity = checker.GetAbsoluteVelocity(JointType.HandLeft);

            if (index >= LOWER_BOUND_TO_BEGIN)
            {
                // Im Zoom-Modus müssen die Hände vor dem Körper sein und dürfen eine Geschwindigkeit nicht übertreten
                if (rightHandOrientation.Contains(Direction.forward)
                    && leftHandOrientation.Contains(Direction.forward)
                    && (rightHandVelocity <= UPPER_BOUND_FOR_VELOCITY)
                    && (leftHandVelocity <= UPPER_BOUND_FOR_VELOCITY))
                {
                    fireSucceeded(this, new EventArgs());
                    Console.WriteLine("Success! Velocity: right " + rightHandVelocity + ", left " + leftHandVelocity);
                }
                else
                {
                    index = 0;
                    fireFailed(this, new EventArgs());
                    Console.WriteLine("Failed during Gesture.");
                }
            }
            else // Steady-Position muss gehalten werden um den Zoom zu beginnen
            {
                // Steady-Position: Beide Hände vor dem Körper und keine Bewegung
                if (rightHandOrientation.Contains(Direction.forward)
                    && leftHandOrientation.Contains(Direction.forward)
                    && rightHandMovement.Contains(Direction.none)
                    && leftHandMovement.Contains(Direction.none))
                {
                    index++;
                    Console.WriteLine("Index++");
                }
                else
                {
                    index = 0;
                    fireFailed(this, new EventArgs());
                    Console.WriteLine("Failed to Begin.");
                }
            }

        }
    }
}
