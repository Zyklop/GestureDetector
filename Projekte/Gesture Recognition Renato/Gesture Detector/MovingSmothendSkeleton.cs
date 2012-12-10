using Microsoft.Kinect;

namespace MF.Engineering.MF8910.GestureDetector.DataSources
{
    class MovingSmothendSkeleton: SmothendSkeleton
    {
        public MovingSmothendSkeleton(Skeleton s, long timestamp)
            : base(s, timestamp)
        {
        }
    }
}
