using System;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    public class AccelerationEventArgs: EventArgs
    {
        public AccelerationEventArgs(double amount)
        {
            Amount = amount;
        }

        public double Amount { get; private set; }
    }
}
