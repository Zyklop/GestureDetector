using MF.Engineering.MF8910.GestureDetector.Gestures;
using System;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    /// <summary>
    /// Details about a gesture event.</summary>
    public abstract class GestureEventArgs: EventArgs
    {

    }

    /// <summary>
    /// Details about a failing gesture part</summary>
    public class FailedGestureEventArgs: GestureEventArgs
    {
        public Condition Condition { get; set; }
    }
}
