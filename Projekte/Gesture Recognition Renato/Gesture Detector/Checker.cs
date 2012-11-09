using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using MF.Engineering.MF8910.GestureDetector.DataSources;

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
            return SkeletonMath.DistanceBetweenPoints(person.CurrentSkeleton.GetPosition(type),person.GetLastSkeleton(1).GetPosition(type)) / (person.MillisBetweenFrames(1,0)/1000);
        }

        public double GetRelativeVelocity(JointType t1, JointType t2)
        {
            if (person.GetLastSkeleton(1) == null)
            {
                return 0;
            }
            double d1 = SkeletonMath.DistanceBetweenPoints(person.CurrentSkeleton.GetPosition(t1), person.CurrentSkeleton.GetPosition(t2));
            double d2 = SkeletonMath.DistanceBetweenPoints(person.GetLastSkeleton(1).GetPosition(t1), person.GetLastSkeleton(1).GetPosition(t2));
            return Math.Abs(d1 - d2) / (person.MillisBetweenFrames(1, 0) / 1000);
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
            throw new NotImplementedException();
        }

        public List<Direction> GetRelativePosition(JointType from, JointType to)
        {
            return SkeletonMath.DirectionTo(person.CurrentSkeleton.GetPosition(from), person.CurrentSkeleton.GetPosition(to));
        }
    }
}
