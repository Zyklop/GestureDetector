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
            if (!hasSkeleton(1))
            {
                return 0;
            }
            if (!hasSkeleton(3))
            {
                return SimpleAbsoluteVelocity(type, 0, 1);
            }
            return SkeletonMath.Median(SimpleAbsoluteVelocity(type, 0, 1), SimpleAbsoluteVelocity(type, 1, 2), SimpleAbsoluteVelocity(type, 2,3 ));
        }

        private double SimpleAbsoluteVelocity(JointType type, int firstTime, int secondTime)
        {
            if (!hasSkeleton(firstTime) || !hasSkeleton(secondTime))
            {
                throw new ArgumentException("No Skeleton at this Index");
            }
            return SkeletonMath.DistanceBetweenPoints(person.GetLastSkeleton(firstTime).GetPosition(type), person.GetLastSkeleton(secondTime).GetPosition(type)) * 1000.0 / (double)(person.MillisBetweenFrames(1, 0));
        }

        public double GetRelativeVelocity(JointType steady, JointType moving)
        {
            if (!hasSkeleton(1))
            {
                return 0;
            }
            SkeletonPoint d0 = SubstractedPointsAt(steady, moving, 0);
            SkeletonPoint d1 = SubstractedPointsAt(steady, moving, 1);
            if (!hasSkeleton(3))
            {
                return SkeletonMath.DistanceBetweenPoints(d0, d1) * 1000.0 / (double)(person.MillisBetweenFrames(1, 0));
            }
            SkeletonPoint d2 = SubstractedPointsAt(steady, moving, 2);
            SkeletonPoint d3 = SubstractedPointsAt(steady, moving, 3);
            return SkeletonMath.Median(
                SkeletonMath.DistanceBetweenPoints(d0, d1) * 1000.0 / (double)(person.MillisBetweenFrames(1, 0)),
                SkeletonMath.DistanceBetweenPoints(d1, d2) * 1000.0 / (double)(person.MillisBetweenFrames(2, 1)),
                SkeletonMath.DistanceBetweenPoints(d2, d3) * 1000.0 / (double)(person.MillisBetweenFrames(3, 2))
                );
        }

        private bool hasSkeleton(int time)
        {
            return person.GetLastSkeleton(time) != null;
        }

        private SkeletonPoint SubstractedPointsAt(JointType steady, JointType moving, int time)
        {
            if (!hasSkeleton(time))
            {
                throw new ArgumentException("No Skeleton at this Index");
            }
            return SkeletonMath.SubstractPoints(person.GetLastSkeleton(time).GetPosition(moving), person.GetLastSkeleton(time).GetPosition(steady));
        }

        public double GetDistanceMedian(JointType t1, JointType t2)
        {
            if (!hasSkeleton(2))
            {
                return GetDistance(t1, t2);
            }
            return SkeletonMath.Median(GetDistance(t1,t2), GetDistance(t1,t2,1), GetDistance(t1,t2,2));
        }

        public double GetDistance(JointType t1, JointType t2)
        {
            return GetDistance(t1, t2, 0);
        }

        private double GetDistance(JointType t1, JointType t2, int time)
        {
            if (!hasSkeleton(time))
            {
                throw new ArgumentException("No Skeleton at this Index");
            }
            return SkeletonMath.DistanceBetweenPoints(person.GetLastSkeleton(time).GetPosition(t1), person.GetLastSkeleton(time).GetPosition(t2));
        }

        public List<Direction> GetAbsoluteMovement(JointType type)
        {
            if (!hasSkeleton(1))
            {
                return new List<Direction>() { Direction.none };
            }
            return SkeletonMath.DirectionTo(person.GetLastSkeleton(1).GetPosition(type), person.CurrentSkeleton.GetPosition(type));
        }

        public List<Direction> GetRelativeMovement(JointType steady, JointType moving)
        {
            if (!hasSkeleton(1))
            {
                return new List<Direction>() { Direction.none };
            }
            return SkeletonMath.DirectionTo(SkeletonMath.SubstractPoints(person.GetLastSkeleton(1).GetPosition(moving), person.GetLastSkeleton(1).GetPosition(steady)),
                SkeletonMath.SubstractPoints(person.CurrentSkeleton.GetPosition(moving), person.CurrentSkeleton.GetPosition(steady)));
        }

        public List<Direction> GetRelativePosition(JointType from, JointType to)
        {
            return SkeletonMath.DirectionTo(person.CurrentSkeleton.GetPosition(from), person.CurrentSkeleton.GetPosition(to));
        }

        public IEnumerable<Direction> GetSteadyAbsoluteMovement(JointType type, int duration)
        {
            List<SkeletonPoint> from = new List<SkeletonPoint>(), to = new List<SkeletonPoint>();
            if (duration < 1)
            {
                throw new ArgumentException("Duration must be at least 1");
            }
            for (int i = 0; i < duration && hasSkeleton(i); i++)
			{
                to.Add(person.GetLastSkeleton(i).GetPosition(type));
			    from.Add(person.GetLastSkeleton(i+1).GetPosition(type));
			}
            return SkeletonMath.SteadyDirectionTo(from, to);
        }

        public IEnumerable<Direction> GetSteadyRelativeMovement(JointType steady, JointType moving, int duration)
        {
            List<SkeletonPoint> from = new List<SkeletonPoint>(), to = new List<SkeletonPoint>();
            if (duration < 1)
            {
                throw new ArgumentException("Duration must be at least 1");
            }
            for (int i = 0; i < duration && hasSkeleton(i); i++)
            {
                to.Add(SkeletonMath.SubstractPoints(person.GetLastSkeleton(i).GetPosition(moving), person.GetLastSkeleton(i).GetPosition(steady)));
                from.Add(SkeletonMath.SubstractPoints(person.GetLastSkeleton(i+1).GetPosition(moving), person.GetLastSkeleton(i+1).GetPosition(steady)));
            }
            return SkeletonMath.SteadyDirectionTo(from, to);
        }

        public IEnumerable<Direction> GetSteadyPosition(JointType from, JointType to, int duration)
        {
            List<SkeletonPoint> origin = new List<SkeletonPoint>(), target = new List<SkeletonPoint>();
            if (duration < 0)
            {
                throw new ArgumentException("Duration must be at least 1");
            }
            for (int i = 0; i <= duration && hasSkeleton(i); i++)
            {
                target.Add(person.GetLastSkeleton(i).GetPosition(to));
                origin.Add(person.GetLastSkeleton(i).GetPosition(from));
            }
            return SkeletonMath.SteadyDirectionTo(origin, target);
        }
    }
}
