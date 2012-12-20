using System;

namespace MF.Engineering.MF8910.GestureDetector.Events
{
    /// <summary>
    /// Gets fired when the devices sensor has other readings. Ignores skeletons.
    /// </summary>
    public class AccelerationEventArgs: EventArgs
    {
        public AccelerationEventArgs(double amount)
        {
            Amount = amount;
        }

        public double Amount { get; private set; }
    }
}
