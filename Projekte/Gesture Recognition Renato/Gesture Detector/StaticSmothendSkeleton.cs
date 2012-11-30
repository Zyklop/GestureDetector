using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace MF.Engineering.MF8910.GestureDetector.DataSources
{
    class StaticSmothendSkeleton:SmothendSkeleton
    {
        public StaticSmothendSkeleton(Skeleton s, long timestamp)
            : base(s, timestamp)
        {
        }
    }
}
