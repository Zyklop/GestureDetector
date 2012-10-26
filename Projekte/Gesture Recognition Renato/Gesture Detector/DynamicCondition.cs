using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;
using GestureEvents;

namespace Conditions
{
    abstract class DynamicCondition: Condition
    {
        protected double startTime;

        public DynamicCondition(Person p): base(p)
        { 
        }

        #region Events

        /**
         * Bewegte Gesten werden erst nach einer gewissen Zeit mit Success quittiert.
         * Davor werden sie dem Checker mit Triggered bekanntgemacht
         * 
         * Gestenablauf:
         * 
         * -------|----------|----------|-----|------|--------|----->
         *        T          T          T     S      T        T
         * 
         * T = fireTriggered
         * S = fireSuccess
         */
        public event EventHandler<EventArgs> Triggered;

        protected void fireTriggered(object sender, EventArgs e)
        {
            Triggered(sender, e);
        }

        #endregion
    }
}
