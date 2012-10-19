using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;

namespace Conditions
{
    class ICondition
    {
        protected Person person;

        public ICondition(Person p) { person = p; }

        public event EventHandler Succeded;

        protected virtual void fireSucceded(object src, EventArgs e)
        {
            Succeded(src, e);
        }

        public event EventHandler Failed;

        protected virtual void fireFailed(object src, EventArgs e)
        {
            Failed(src, e);
        }
    }
}
