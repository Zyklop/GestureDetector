using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureEvents
{
    public class AccelerationEventArgs:EventArgs
    {
        private double amount;

        public AccelerationEventArgs(double Amount)
        {
            this.amount = Amount;
        }

        public double Amount { get { return amount; } }
    }
}
