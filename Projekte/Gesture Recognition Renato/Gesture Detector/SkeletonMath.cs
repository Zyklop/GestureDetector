using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conditions
{
    public enum Direction
    {
        forward, upward, downward, left, right, backward, none
    }

    class SkeletonMath
    {
        public const double TOLERANCE = 0.1;

        /**
         * Distanz in Metern 
         */
        public static double DistanceBetweenPoints(SkeletonPoint p1, SkeletonPoint p2)
        {
            double dx = Math.Abs(p2.X - p1.X);
            double dy = Math.Abs(p2.Y - p1.Y);
            double dz = Math.Abs(p2.Z - p1.Z);
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static List<Direction> DirectionTo(SkeletonPoint from, SkeletonPoint to)
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
