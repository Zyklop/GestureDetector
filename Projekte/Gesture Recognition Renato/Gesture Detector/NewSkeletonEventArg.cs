using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    public class NewSkeletonEventArgs:EventArgs
    {
        SmothendSkeleton ske;

        public NewSkeletonEventArgs(SmothendSkeleton skel)
        {
            ske = skel;
        }

        public SmothendSkeleton Skeleton { get { return ske; } }
    }
}
