using System.Collections.Generic;
using MF.Engineering.MF8910.GestureDetector.DataSources;


namespace MF.Engineering.MF8910.GestureDetector.Gestures.Wave
{
    class WaveGestureChecker: GestureChecker
    {
        protected const int ConditionTimeout = 2500;

        public WaveGestureChecker(Person p)
            : base(new List<Condition> {

                new WaveRightCondition(p),
                new WaveLeftCondition(p)

            }, ConditionTimeout) { }
    }
}
