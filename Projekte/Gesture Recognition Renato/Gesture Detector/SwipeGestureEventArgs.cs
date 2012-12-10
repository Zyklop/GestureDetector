using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Tools;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Swipe
{
    public class SwipeGestureEventArgs: GestureEventArgs
    {
        public Direction Direction { get; set; }
    }
}
