using MF.Engineering.MF8910.GestureDetector.DataSources;
using Microsoft.Kinect;
using System.Linq;
using System.Collections.Generic;
using MF.Engineering.MF8910.GestureDetector.Tools;
using MF.Engineering.MF8910.GestureDetector.Events;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Wave
{
    class WaveRightCondition: DynamicCondition
    {
        private int _index;
        private Checker checker;
        List<Direction> _handToHeadDirections;

        public WaveRightCondition(Person p)
            : base(p)
        {
            _index = 0;
            checker = new Checker(p);
        }

        protected override void Check(object sender, NewSkeletonEventArgs e)
        {
            checker.GetAbsoluteMovement(JointType.HandRight);
            _handToHeadDirections = checker.GetRelativePosition(JointType.ShoulderCenter, JointType.HandRight).ToList();
            // Prüfe ob Handbewegung nach links abläuft und ob sich die Hand über dem Kopf befindet
            double handspeed = checker.GetAbsoluteVelocity(JointType.HandRight);
            //Debug.WriteLine(handspeed);
            // min required speed
            if (handspeed < 2)
            {
                _index = 0;
            }
            // hand must be right
            if (_index == 0 && _handToHeadDirections.Contains(Direction.Right))
            {
                _index = 1;
            }
            // hand is on top
            else if (_index == 1 && _handToHeadDirections.Contains(Direction.Upward))
            {
                _index = 2;
            }
            //hand is left
            else if (_index == 2 && _handToHeadDirections.Contains(Direction.Left))
            {
                FireSucceeded(this, new WaveGestureEventArgs());
                //Debug.WriteLine("triggered" + e.Skeleton.Timestamp);
                _index = 0;
                //if (index >= LOWER_BOUND_FOR_SUCCESS)
                //{
                //    fireSucceeded(this, null);
                //}

            }
        }
    }
}
