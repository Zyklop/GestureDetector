using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Engineering.MF8910.GestureDetector.Tools
{
    public enum Direction
    {
        forward, upward, downward, left, right, backward, none
    }

    class SkeletonMath
    {
        public const double TOLERANCE = 0.06;
        public const double MEDIANTOLERANCE = 0.01;
        public const double MEDIANCORRECTNEEDED = 0.66666666;

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

        public static SkeletonPoint SubstractPoints(SkeletonPoint op1, SkeletonPoint op2)
        {
            SkeletonPoint res = new SkeletonPoint();
            res.X = op1.X - op2.X;
            res.Y = op1.Y - op2.Y;
            res.Z = op1.Z - op2.Z;
            return res;
        }

        public static SkeletonPoint AddPoints(SkeletonPoint op1, SkeletonPoint op2)
        {
            SkeletonPoint res = new SkeletonPoint();
            res.X = op1.X + op2.X;
            res.Y = op1.Y + op2.Y;
            res.Z = op1.Z + op2.Z;
            return res;
        }

        public static double Median(double d1, double d2, double d3)
        {
            // more performance than copying and sorting
            if ((d1 > d2 && d1 < d3) || (d1 < d2 && d1 > d3))
            {
                return d1;
            }
            if ((d2 > d1 && d2 < d3) || (d2 < d1 && d2 > d3))
            {
                return d2;
            }
            return d3;
        }

        public static double Median(IEnumerable<double> values)
        {
            List<double> d = new List<double>();
            d.AddRange(values);
            d.Sort();
            return d[d.Count / 2];
        }

        public static List<Direction> SteadyDirectionTo(IEnumerable<SkeletonPoint> from, IEnumerable<SkeletonPoint> to)
        {
            List<List<Direction>> directions = new List<List<Direction>>();
            var origin = from.ToList();
            var target = to.ToList();
            if (from.Count() != to.Count())
            {
                throw new ArgumentException("Length not identical");
            }
            for (int i = 0; i < from.Count(); i++)
            {
                directions.Add(new List<Direction>());
                double dx = target[i].X - origin[i].X;
                double dy = target[i].Y - origin[i].Y;
                double dz = target[i].Z - origin[i].Z;
                if (dx > TOLERANCE)
                {
                    directions[i].Add(Direction.right);
                }
                else if (dx < -TOLERANCE)
                {
                    directions[i].Add(Direction.left);
                }
                if (dy > TOLERANCE)
                {
                    directions[i].Add(Direction.upward);
                }
                else if (dy < -TOLERANCE)
                {
                    directions[i].Add(Direction.downward);
                }
                if (dz > TOLERANCE)
                {
                    directions[i].Add(Direction.backward);
                }
                else if (dz < -TOLERANCE)
                {
                    directions[i].Add(Direction.forward);
                }
                
            }
            List<Direction> res = new List<Direction>();
            foreach (Direction item in Enum.GetValues(typeof(Direction)))
            {
                if (directions.Where(x => x.Contains(item)).Count() > origin.Count * MEDIANCORRECTNEEDED)
                {
                    res.Add(item);
                }
            }
            if (res.Count == 0)
            {
                res.Add(Direction.none);
            }
            return res;
        }

        public static List<Direction> DirectionTo(SkeletonPoint from, SkeletonPoint to)
        {
            List<Direction> res = new List<Direction>();
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double dz = to.Z - from.Z;
            if (dx > TOLERANCE)
            {
                res.Add(Direction.right);
            }
            else if (dx < -TOLERANCE)
            {
                res.Add(Direction.left);
            }
            if (dy > TOLERANCE)
            {
                res.Add(Direction.upward);
            }
            else if (dy < -TOLERANCE)
            {
                res.Add(Direction.downward);
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
