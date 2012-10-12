using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;

namespace Conditions
{
    abstract class ICondition
    {
        protected Person person;

        public ICondition(Person p) { person = p; }
        public abstract bool check();

        public event EventHandler Succeded;

        public event EventHandler Failed;
    }
}
