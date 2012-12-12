using System;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    /// <summary>
    /// New skeletons for Kinect arrived
    /// </summary>
    public class NewSkeletonEventArgs: EventArgs
    {
        public NewSkeletonEventArgs(SmothendSkeleton skeleton)
        {
            Skeleton = skeleton;
        }

        public SmothendSkeleton Skeleton { get; private set; }
    }
}
