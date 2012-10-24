using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSources;

namespace Gesture_Detector
{
    class PersonPassiveEventArgs:EventArgs
    {
        private Person p;

        public PersonPassiveEventArgs(Person person)
        {
            p = person;
        }

        public Person Person { get { return p; } }
    }
}
