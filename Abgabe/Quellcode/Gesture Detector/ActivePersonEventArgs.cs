using System;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    /// <summary>
    /// A new person got active
    /// </summary>
    public class ActivePersonEventArgs:EventArgs
    {
        /// <summary>
        /// The new Person
        /// </summary>
        public Person Person { get; private set; }

        public ActivePersonEventArgs(Person p)
        {
            Person = p;
        }
    }
}
