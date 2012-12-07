using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;


namespace MF.Engineering.MF8910.GestureDetector.Gestures.Wave
{
    class WaveGestureChecker: GestureChecker
    {
        protected const int CONDITION_TIMEOUT = 2500;

        public WaveGestureChecker(Person p)
            : base(new List<Condition> {

                new WaveRightCondition(p),
                new WaveLeftCondition(p),

            }, CONDITION_TIMEOUT) { }
    }
}
