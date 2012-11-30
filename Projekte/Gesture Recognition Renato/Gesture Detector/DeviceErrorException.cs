using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace MF.Engineering.MF8910.GestureDetector.Exceptions
{
    class DeviceErrorException:Exception
    {

        public DeviceErrorException(string p):base(p)
        {
        }
        public KinectStatus Status { get; set; }
    }
}
