using System;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    public class ActivePersonEventArgs:EventArgs
    {
        public Person Person { get; private set; }

        public ActivePersonEventArgs(Person p)
        {
            Person = p;
        }
    }
}
