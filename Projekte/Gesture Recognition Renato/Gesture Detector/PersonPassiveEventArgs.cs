using System;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    public class PersonPassiveEventArgs:EventArgs
    {
        public PersonPassiveEventArgs(Person person)
        {
            Person = person;
        }

        public Person Person { get; private set; }
    }
}
