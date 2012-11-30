using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Engineering.MF8910.GestureDetector.DataSources
{
    static class CurrentMillis
    {
        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long Millis { get { return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds); } }
    }
}