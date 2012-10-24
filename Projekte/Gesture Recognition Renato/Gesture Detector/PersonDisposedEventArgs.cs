using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSources;

namespace Gesture_Detector
{
    class PersonDisposedEventArgs:EventArgs
    {
        private Person p;

        public PersonDisposedEventArgs(Person person)
        {
            p = person;
        }

        public Person Person { get { return p; } }
    }
}
