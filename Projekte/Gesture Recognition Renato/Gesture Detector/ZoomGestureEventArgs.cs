using MF.Engineering.MF8910.GestureDetector.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Zoom
{
    class ZoomGestureEventArgs: GestureEventArgs
    {
        public double Gauge { get; set; }
    }
}
