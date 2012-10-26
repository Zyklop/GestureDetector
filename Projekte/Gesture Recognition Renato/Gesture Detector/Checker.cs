using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;
using Microsoft.Kinect;

namespace Conditions
{
    public enum Direction
    {
       forward, upward, downward, left, right, backward, none
   }

    class Checker
    {
        public const double TOLERANCE= 0.1;

        private Person person;

        public Checker(ref Person p)
        {
            person = p;
        }

        public double GetAbsoluteVelocity(JointType type)
        {
            return DistanceBetweenPoints(person.CurrentSkeleton.GetPosition(type),person.GetLastSkeleton(1).GetPosition(type)) / (person.MillisBetweenFrames(1,0)/1000);
        }

        public double GetRelativeVelocity(JointType t1, JointType t2)
        {
            double d1 = DistanceBetweenPoints(person.CurrentSkeleton.GetPosition(t1), person.CurrentSkeleton.GetPosition(t2));
            double d2 = DistanceBetweenPoints(person.GetLastSkeleton(1).GetPosition(t1), person.GetLastSkeleton(1).GetPosition(t2));
            return Math.Abs(d1 - d2) / (person.MillisBetweenFrames(1, 0) / 1000);
        }

        public double GetDistance(JointType t1, JointType t2)
        {
            return DistanceBetweenPoints(person.CurrentSkeleton.GetPosition(t1), person.CurrentSkeleton.GetPosition(t2));
        }

        public List<Direction> GetAbsoluteMovement(JointType type)
        {
            return DirectionTo(person.CurrentSkeleton.GetPosition(type), person.GetLastSkeleton(1).GetPosition(type));
        }

        public List<Direction> GetRelativeMovement(JointType steady, JointType moving)
        {
            throw new NotImplementedException();
        }

        public List<Direction> GetRelativePosition(JointType from, JointType to)
        {
            return DirectionTo(person.CurrentSkeleton.GetPosition(from), person.CurrentSkeleton.GetPosition(to));
        }

        private double DistanceBetweenPoints(SkeletonPoint p1, SkeletonPoint p2)
        {
            double dx = Math.Abs(p2.X - p1.X);
            double dy = Math.Abs(p2.Y - p1.Y);
            double dz = Math.Abs(p2.Z - p1.Z);
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        private List<Direction> DirectionTo(SkeletonPoint from, SkeletonPoint to)
        {
            List<Direction> res = new List<Direction>();
            double dx = from.X - to.X;
            double dy = from.Y - to.Y;
            double dz = from.Z - to.Z;
            if (dx > TOLERANCE)
            {
                res.Add(Direction.downward);
            }
            else if (dx < -TOLERANCE)
            {
                res.Add(Direction.upward);
            }
            if (dy > TOLERANCE)
            {
                res.Add(Direction.left);
            }
            else if (dy < -TOLERANCE)
            {
                res.Add(Direction.right);
            }
            if (dz > TOLERANCE)
            {
                res.Add(Direction.backward);
            }
            else if (dz < -TOLERANCE)
            {
                res.Add(Direction.forward);
            }
            if (res.Count == 0)
            {
                res.Add(Direction.none);
            }
            return res;
        }
    }
}
