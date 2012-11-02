using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSources;

namespace GestureEvents
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
