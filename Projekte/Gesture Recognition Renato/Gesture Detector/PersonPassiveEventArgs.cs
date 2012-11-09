using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    public class PersonPassiveEventArgs:EventArgs
    {
        private Person p;

        public PersonPassiveEventArgs(Person person)
        {
            p = person;
        }

        public Person Person { get { return p; } }
    }
}
