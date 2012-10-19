using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace DataSources
{
    class StaticSmothendSkeleton:SmothendSkeleton
    {
        public StaticSmothendSkeleton(Skeleton s) : base(s)
        {
        }
    }
}
