using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gesture_Detection_Api
{
    class Person
    {
        private bool active;
        private Device device;
        private GestureChecker gestureChecker;

        public delegate void GestureSucceededEventHandler(object sender, EventArgs data);
        public event GestureSucceededEventHandler OnWave;

        public Person(Device deviceOnWhichPersonIsRecognized)
        {
            device = deviceOnWhichPersonIsRecognized;
            device.OnPersonApears += new EventHandler();
            // init Gesten
        }
        
        public bool Active { get; set; }

        

    }
}
