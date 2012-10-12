using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;

namespace Conditions
{
    class HandOverHeadCondition:ICondition
    {
        public HandOverHeadCondition(ref Person p)
            : base(p)
        {
        }

        public override bool check()
        {
            return false;
        }
    }
}
