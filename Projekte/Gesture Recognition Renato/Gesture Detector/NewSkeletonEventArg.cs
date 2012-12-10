using System;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    public class NewSkeletonEventArgs: EventArgs
    {
        public NewSkeletonEventArgs(SmothendSkeleton skeleton)
        {
            Skeleton = skeleton;
        }

        public SmothendSkeleton Skeleton { get; private set; }
    }
}
