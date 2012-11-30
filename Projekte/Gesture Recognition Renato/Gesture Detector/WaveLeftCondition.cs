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
namespace MF.Engineering.MF8910.GestureDetector.Gestures.Wave
{
    public class WaveLeftCondition: DynamicCondition
    {
        private const int LOWER_BOUND_FOR_SUCCESS = 3;
        private int index = 0;
        private Checker checker;
        List<Direction> rightHandDirections, handToHeadDirections;

        public WaveLeftCondition(Person p)
            : base(p)
        {
            checker = new Checker(p);
        }

        protected override void check(object sender, NewSkeletonEventArgs e)
        {
            //rightHandDirections = checker.GetAbsoluteMovement(JointType.HandRight);
            handToHeadDirections = checker.GetRelativePosition(JointType.Head, JointType.HandRight);
            double handspeed = checker.GetAbsoluteVelocity(JointType.HandRight);
            //Debug.WriteLine(handspeed);
            // min required speed
            if (handspeed < 2)
            {
                index = 0;
            }
            // hand must be left
            if (index == 0 && handToHeadDirections.Contains(Direction.right))
            {
                index = 1;
            }
                // hand is on top
            else if (index == 1 && handToHeadDirections.Contains(Direction.upward))
            {
                index = 2;
            }
                //hand is right
            else if (index == 2 && handToHeadDirections.Contains(Direction.right))
            {
                fireSucceeded(this, null);
                //Debug.WriteLine("triggered");
                index = 0;
                //if (index >= LOWER_BOUND_FOR_SUCCESS)
                //{
                //    fireSucceeded(this, null);
                //}
                
            }
        }
    }
}

