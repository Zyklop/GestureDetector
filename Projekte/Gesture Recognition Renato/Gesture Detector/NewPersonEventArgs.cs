using System;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    /// <summary>
    /// A new person was detected, passive
    /// </summary>
    public class NewPersonEventArgs: EventArgs
    {
        public Person Person { get; private set; }

        public NewPersonEventArgs(Person p)
        {
            Person = p;
        }

    }
}
