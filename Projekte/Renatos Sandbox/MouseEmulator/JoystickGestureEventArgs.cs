using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MF.Engineering.MF8910.GestureDetector.Events;

namespace MouseEmulator
{
    class JoystickGestureEventArgs : GestureEventArgs
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double DistToShoulderZ { get; set; }
    }
}
