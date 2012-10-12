using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Kinect;
using KinectSkeletonTracker.Gestures;
using KinectSkeletonTracker.Gestures.GestureParts;
using KinectSkeletonTracker;
using System.Diagnostics;

namespace GestureTryOne
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

            IRelativeGestureSegment[] waveLeftSegments = new IRelativeGestureSegment[20];
            WaveLeftSegment1 waveLeftSegment = new WaveLeftSegment1();
            for (int i = 0; i < 20; i++)
            {
                // gesture consists of the same thing 10 times 
                waveLeftSegments[i] = waveLeftSegment;
            }

            GestureController g = new GestureController();
            g.AddGesture(GestureType.WaveLeft, waveLeftSegments);
            g.GestureRecognised += OnGestureRecognized;

            Console.WriteLine("start recognizing:");
            Skeleton[] skeletons = new Skeleton[6];
            while (true)
            {
                SkeletonFrame f = Dev.SkeletonStream.OpenNextFrame(200);
                if (f != null)
                {
                    f.CopySkeletonDataTo(skeletons);
                    foreach (var skeleton in skeletons)
                    {
                        // skip the skeleton if it is not being tracked
                        if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                            continue;

                        // update the gesture controller
                        g.UpdateAllGestures(skeleton);
                    }
                }
            }
        }

        private static void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            switch (e.GestureType)
            {
                case GestureType.WaveLeft:
                    Console.WriteLine("Linke Hand winkt!");
                    break;
                default:
                    break;
            }
        }

        public static void GetKinect()
        {
            Dev = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }
    }
}
