using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;

namespace GestureEvents
{
    class SkeletonsReadyEventArg:EventArgs
    {
        private List<Match> matches;

        public SkeletonsReadyEventArg(List<Match> ml)
        {
            matches = ml;
        }

        public SmothendSkeleton GetSkeleton(Person p)
        {
            foreach (Match m in matches)
            {
                if (m.Person==p)
                {
                    return m.Skeleton;
                }
            }
            return null;
        }
    }
}
