using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSources;

namespace GestureEvents
{
    public class NewSkeletonEventArg:EventArgs
    {
        SmothendSkeleton ske;

        public NewSkeletonEventArg(SmothendSkeleton skel)
        {
            ske = skel;
        }

        public SmothendSkeleton Skeleton { get { return ske; } }
    }
}
