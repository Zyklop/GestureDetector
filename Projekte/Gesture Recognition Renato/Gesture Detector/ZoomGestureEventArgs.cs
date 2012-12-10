using MF.Engineering.MF8910.GestureDetector.Events;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Zoom
{
    public class ZoomGestureEventArgs : GestureEventArgs
    {
        public double ZoomFactorFromBegin { get; set; }
        public double ZoomFactorFromLast { get; set; }
    }
}
