using MF.Engineering.MF8910.GestureDetector.DataSources;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MF.Engineering.MF8910.GestureDetector.Tools;
using MF.Engineering.MF8910.GestureDetector.Events;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Wave
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
            handToHeadDirections = checker.GetRelativePosition(JointType.ShoulderCenter, JointType.HandRight);
            // Prüfe ob Handbewegung nach links abläuft und ob sich die Hand über dem Kopf befindet
            double handspeed = checker.GetAbsoluteVelocity(JointType.HandRight);
            //Debug.WriteLine(handspeed);
            // min required speed
            if (handspeed < 2)
            {
                index = 0;
            }
            // hand must be right
            if (index == 0 && handToHeadDirections.Contains(Direction.right))
            {
                index = 1;
            }
            // hand is on top
            else if (index == 1 && handToHeadDirections.Contains(Direction.upward))
            {
                index = 2;
            }
            //hand is left
            else if (index == 2 && handToHeadDirections.Contains(Direction.left))
            {
                fireSucceeded(this, null);
                //Debug.WriteLine("triggered" + e.Skeleton.Timestamp);
                index = 0;
                //if (index >= LOWER_BOUND_FOR_SUCCESS)
                //{
                //    fireSucceeded(this, null);
                //}

            }
        }
    }
}
