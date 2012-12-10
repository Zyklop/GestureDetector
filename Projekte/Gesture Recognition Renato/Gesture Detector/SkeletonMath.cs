using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MF.Engineering.MF8910.GestureDetector.Tools
{
    public enum Direction
    {
        Forward, Upward, Downward, Left, Right, Backward, None
    }

    /// <summary>
    /// Library for independent vector aritmetics</summary>
    class SkeletonMath
    {
        public const double Tolerance = 0.06;
        public const double MedianTolerance = 0.01;
        public const double MedianCorrectNeeded = 0.66666666;

        /// <summary>
        /// Get distance of two skeleton points in meters.</summary>
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
                if (dx > Tolerance)
                {
                    directions[i].Add(Direction.Right);
                }
                else if (dx < -Tolerance)
                {
                    directions[i].Add(Direction.Left);
                }
                if (dy > Tolerance)
                {
                    directions[i].Add(Direction.Upward);
                }
                else if (dy < -Tolerance)
                {
                    directions[i].Add(Direction.Downward);
                }
                if (dz > Tolerance)
                {
                    directions[i].Add(Direction.Backward);
                }
                else if (dz < -Tolerance)
                {
                    directions[i].Add(Direction.Forward);
                }
                
            }
            List<Direction> res = new List<Direction>();
            foreach (Direction item in Enum.GetValues(typeof(Direction)))
            {
                if (directions.Count(x => x.Contains(item)) > origin.Count * MedianCorrectNeeded)
                {
                    res.Add(item);
                }
            }
            if (res.Count == 0)
            {
                res.Add(Direction.None);
            }
            return res;
        }

        /// <summary>
        /// Get an abstract direction type between two skeleton points</summary>
        /// <param name="from">
        /// Source Point</param>
        /// <param name="to">
        /// Target Point</param>
        /// <returns>
        /// Returns a list of three directions (for each axis)</returns>
        public static List<Direction> DirectionTo(SkeletonPoint from, SkeletonPoint to)
        {
            List<Direction> res = new List<Direction>();
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double dz = to.Z - from.Z;
            if (dx > Tolerance)
            {
                res.Add(Direction.Right);
            }
            else if (dx < -Tolerance)
            {
                res.Add(Direction.Left);
            }
            if (dy > Tolerance)
            {
                res.Add(Direction.Upward);
            }
            else if (dy < -Tolerance)
            {
                res.Add(Direction.Downward);
            }
            if (dz > Tolerance)
            {
                res.Add(Direction.Backward);
            }
            else if (dz < -Tolerance)
            {
                res.Add(Direction.Forward);
            }
            if (res.Count == 0)
            {
                res.Add(Direction.None);
            }
            return res;
        }
    }
}
