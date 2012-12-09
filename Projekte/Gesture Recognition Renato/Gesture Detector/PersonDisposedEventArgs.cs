using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    public class PersonDisposedEventArgs: EventArgs
    {
        private Person person;

        public PersonDisposedEventArgs(Person person)
        {
            this.person = person;
        }

        public Person Person { get { return person; } }
    }
}
