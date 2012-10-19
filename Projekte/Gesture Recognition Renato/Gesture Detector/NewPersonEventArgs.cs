using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;

namespace GestureEvents
{
    class NewPersonEventArgs:EventArgs
    {
        private Person pers;

        public Person Person { get{return pers;}}

        public NewPersonEventArgs(Person p)
        {
            pers = p;
        }

    }
}
