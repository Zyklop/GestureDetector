using MF.Engineering.MF8910.GestureDetector.Gestures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    public abstract class GestureEventArgs: EventArgs
    {

    }

    public class FailedGestureEventArgs: GestureEventArgs
    {
        public Condition Condition { get; set;  }
    }
}
