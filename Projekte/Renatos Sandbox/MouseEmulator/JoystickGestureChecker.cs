using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Gestures;

namespace MouseEmulator
{
    class JoystickGestureChecker : GestureChecker
    {
        public JoystickGestureChecker(Person p, bool dynamicMovement)
            : base(new List<Condition> {

                new JoystickCondition(p, dynamicMovement)

            }, 5000)
        {
        }
    }
}
