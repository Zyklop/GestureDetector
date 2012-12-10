using System;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    public class PersonDisposedEventArgs: EventArgs
    {
        public PersonDisposedEventArgs(Person person)
        {
            Person = person;
        }

        public Person Person { get; private set; }
    }
}
