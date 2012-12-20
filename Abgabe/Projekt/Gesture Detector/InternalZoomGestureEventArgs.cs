using MF.Engineering.MF8910.GestureDetector.Events;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Zoom
{
    /// <summary>
    /// EventArgs to pass the hand distance to the GestureChecker
    /// </summary>
    internal class InternalZoomGestureEventArgs: GestureEventArgs
    {
        /// <summary>
        /// Actual distance between hands
        /// </summary>
        public double Gauge { get; set; }
    }
}
