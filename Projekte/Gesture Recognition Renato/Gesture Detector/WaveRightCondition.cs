using DataSources;
using GestureEvents;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Conditions.Login
{
    public class WaveRightCondition: DynamicCondition
    {
        private const int LOWER_BOUND_FOR_SUCCESS = 3;
        private int index = 0;
        private Checker checker;
        List<Direction> rightHandDirections, handToHeadDirections;

        public WaveRightCondition(Person p)
            : base(p)
        {
            checker = new Checker(p);
        }

        protected override void check(object sender, NewSkeletonEventArgs e)
        {
            rightHandDirections = checker.GetAbsoluteMovement(JointType.HandRight);
            handToHeadDirections = checker.GetRelativePosition(JointType.Head, JointType.HandRight);

            // Prüfe ob Handbewegung nach links abläuft und ob sich die Hand über dem Kopf befindet
            if (rightHandDirections.Contains(Direction.right) && handToHeadDirections.Contains(Direction.upward))
            {
                fireTriggered(this, null);
                index++;
                if (index >= LOWER_BOUND_FOR_SUCCESS)
                {
                    fireSucceeded(this, null);
                }
            }
        }
    }
}
