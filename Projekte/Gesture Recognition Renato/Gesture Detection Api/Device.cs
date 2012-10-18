using System;

namespace Gesture_Detection_Api
{
    class Device
    {
        public delegate void EventHandler(object sender, EventArgs data);
        public event EventHandler OnSkeletonAppears;

        public Device()
        {
            kinect.onFrame += delegate {
                if (skeleton)
                {
                    OnSkeletonAppears(this, new EventArgs());
                }
            };
        }

        public Person getActive()
        {
            return new Skeleton();
        }
    }
}
