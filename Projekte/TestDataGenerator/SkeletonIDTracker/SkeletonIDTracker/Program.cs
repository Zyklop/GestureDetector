using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace SkeletonIDTracker
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
            Dev.SkeletonStream.Enable();
            Skeleton[] skeletons = new Skeleton[6];
            while (true)
            {
                SkeletonFrame Frm = Dev.SkeletonStream.OpenNextFrame(100);
                if (Frm != null)
                {
                    Frm.CopySkeletonDataTo(skeletons);
                    for (int i = 0; i < skeletons.Length; i++ )
                    {
                        Skeleton ske = skeletons[i];
                        if (ske.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            Console.Write("ID: " + ske.TrackingId + " index: "+i);
                        }
                    }
                    Console.WriteLine();
                    System.Threading.Thread.Sleep(33);
                }
            }
        }

        public static void GetKinect()
        {
            Dev = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }
    }
}
