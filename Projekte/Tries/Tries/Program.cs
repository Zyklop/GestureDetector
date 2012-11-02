using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Tries
{
    class Program
    {
        private static KinectSensor Dev = KinectSensor.KinectSensors.First();


        static void Main(string[] args)
        {

            GetKinect();
            Dev.Start();
            Console.WriteLine(Dev.IsRunning);
            Dev.ElevationAngle = 5;
            Dev.SkeletonFrameReady += printSkeletonReady;
            System.Threading.Thread.Sleep(10000);
            Dev.SkeletonStream.Enable();
            System.Threading.Thread.Sleep(10000);

        }

        static void printSkeletonReady(object src, SkeletonFrameReadyEventArgs e)
        {
            Console.WriteLine("Frame Ready Event");
        }

        public static void GetKinect()
        {
            Dev = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }
    }
}
