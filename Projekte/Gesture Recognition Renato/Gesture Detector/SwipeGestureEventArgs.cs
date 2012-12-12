using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Tools;

namespace MF.Engineering.MF8910.GestureDetector.Gestures.Swipe
{
    /// <summary>
    /// Person swiped
    /// </summary>
    public class SwipeGestureEventArgs: GestureEventArgs
    {
        /// <summary>
        /// Direction of the swipe
        /// </summary>
        public Direction Direction { get; set; }
    }
}
