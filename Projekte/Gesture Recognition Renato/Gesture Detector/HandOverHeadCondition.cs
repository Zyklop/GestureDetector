using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;
using Microsoft.Kinect;

namespace Conditions
{
    class RightHandOverHeadCondition:ICondition
    {
        Checker c;

        public RightHandOverHeadCondition(ref Person p)
            : base(p)
        {
            c = new Checker(ref p);
            p.NewSkeleton += check;
        }

        void check(Object src, EventArgs e)
        {
            if (c.GetRelativePosition(JointType.Head, JointType.HandRight).Contains(Direction.upward))
            {
                fireSucceded(this, new EventArgs());
            }
            else
            {
                fireFailed(this, new EventArgs());
            }
        }
    }
}
