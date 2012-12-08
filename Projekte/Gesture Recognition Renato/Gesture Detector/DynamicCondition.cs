using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;

namespace MF.Engineering.MF8910.GestureDetector.Gestures
{
    public abstract class DynamicCondition: Condition
    {
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
        public event EventHandler<GestureEventArgs> Triggered;

        protected void fireTriggered(object sender, GestureEventArgs e)
        {
            if (Triggered != null)
            {
                Triggered(sender, e);
            }
        }

        #endregion
    }
}
