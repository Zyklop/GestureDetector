using System;
using Microsoft.Kinect;

namespace MF.Engineering.MF8910.GestureDetector.Exceptions
{
    /// <summary>
    /// Collecting Exception for all device errors.</summary>
    [Serializable]
    public class DeviceErrorException: Exception
    {
        public KinectStatus Status { get; set; }

        public DeviceErrorException(string p):base(p)
        {
        }
    }
}
