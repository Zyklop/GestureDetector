using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures;
using MF.Engineering.MF8910.GestureDetector.Tools;
using Microsoft.Kinect;

namespace MouseEmulator
{
    class JoystickCondition : Condition
    {
        private readonly Person _person;
        private readonly bool _dyn;
        private SkeletonPoint _last;
        private Checker _checker;
        private const double EPSILON = 0.01;
        private const double ApproximationValue = 5.0;


        public JoystickCondition(Person person, bool dyn) : base(person)
        {
            _person = person;
            _checker = new Checker(person);
            _dyn = dyn;
        }

        protected override void Check(object src, NewSkeletonEventArgs e)
        {
            if ((Math.Abs(_last.X - 0.0) < EPSILON && Math.Abs(_last.Y - 0.0) < EPSILON && Math.Abs(_last.Z - 0.0) < EPSILON) || 
                (!_dyn && _checker.GetRelativePosition(JointType.Head, JointType.HandLeft).Contains(Direction.Upward)))
            {
                _last = e.Skeleton.GetPosition(JointType.HandRight);
            }
            else
            {
                double xDiff = e.Skeleton.GetPosition(JointType.HandRight).X - _last.X;
                double yDiff = _last.Y - e.Skeleton.GetPosition(JointType.HandRight).Y;
                double zDiff = _last.Z - e.Skeleton.GetPosition(JointType.HandRight).Z;//e.Skeleton.GetPosition(JointType.ShoulderRight).Z - e.Skeleton.GetPosition(JointType.HandRight).Z;
                if (_dyn)
                {
                    if ((_last.X < e.Skeleton.GetPosition(JointType.ShoulderRight).X - 0.2 && xDiff > 0.0) ||
                        (_last.X > e.Skeleton.GetPosition(JointType.ShoulderRight).X + 0.4 && xDiff < 0.0))
                    {
                        _last.X += (float)(xDiff / ApproximationValue);
                    }
                    if ((_last.Y < e.Skeleton.GetPosition(JointType.ShoulderRight).Y - 0.3 && yDiff > 0.0) ||
                        (_last.Y > e.Skeleton.GetPosition(JointType.ShoulderRight).Y + 0.3 && yDiff < 0.0))
                    {
                        _last.Y += (float)(yDiff / ApproximationValue);
                    }
                }
                FireSucceeded(this, new JoystickGestureEventArgs{X = xDiff, Y = yDiff, DistToShoulderZ = zDiff});
            }
        }
    }
}
