using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    public class ActivePersonEventArgs:EventArgs
    {
        private Person pers;

        public Person Person { get{return pers;}}

        public ActivePersonEventArgs(Person p)
        {
            pers = p;
        }
    }
}
