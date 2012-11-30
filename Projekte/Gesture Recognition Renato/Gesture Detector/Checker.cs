using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using System.Diagnostics;

namespace MF.Engineering.MF8910.GestureDetector.Tools
{
    class Checker
    {
        private Person person;

        public Checker(Person p)
        {
            person = p;
        }

        public double GetAbsoluteVelocity(JointType type)
        {
            if (person.GetLastSkeleton(1) == null)
            {
                return 0;
            }
            return SkeletonMath.DistanceBetweenPoints(person.CurrentSkeleton.GetPosition(type),person.GetLastSkeleton(1).GetPosition(type)) * 1000.0 / (double)(person.MillisBetweenFrames(1,0));
        }

        public double GetRelativeVelocity(JointType steady, JointType moving)
        {
            if (person.GetLastSkeleton(1) == null)
            {
                return 0;
            }
            SkeletonPoint d1 = SkeletonMath.SubstractPoints(person.CurrentSkeleton.GetPosition(moving), person.CurrentSkeleton.GetPosition(steady));
            SkeletonPoint d2 = SkeletonMath.SubstractPoints(person.GetLastSkeleton(1).GetPosition(moving), person.GetLastSkeleton(1).GetPosition(steady));
            return SkeletonMath.DistanceBetweenPoints(d1, d2) / (double)(person.MillisBetweenFrames(1, 0) * 1000.0);
        }

        public double GetDistance(JointType t1, JointType t2)
        {
            return SkeletonMath.DistanceBetweenPoints(person.CurrentSkeleton.GetPosition(t1), person.CurrentSkeleton.GetPosition(t2));
        }

        public List<Direction> GetAbsoluteMovement(JointType type)
        {
            if (person.GetLastSkeleton(1) == null)
            {
                return new List<Direction>() { Direction.none };
            }
            return SkeletonMath.DirectionTo(person.GetLastSkeleton(1).GetPosition(type), person.CurrentSkeleton.GetPosition(type));
        }

        public List<Direction> GetRelativeMovement(JointType steady, JointType moving)
        {
            return SkeletonMath.DirectionTo(SkeletonMath.SubstractPoints(person.GetLastSkeleton(1).GetPosition(moving), person.GetLastSkeleton(1).GetPosition(steady)),
                SkeletonMath.SubstractPoints(person.CurrentSkeleton.GetPosition(moving), person.CurrentSkeleton.GetPosition(steady)));
        }

        public List<Direction> GetRelativePosition(JointType from, JointType to)
        {
            return SkeletonMath.DirectionTo(person.CurrentSkeleton.GetPosition(from), person.CurrentSkeleton.GetPosition(to));
        }
    }
}
