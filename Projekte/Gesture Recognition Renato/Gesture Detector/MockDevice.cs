using MF.Engineering.MF8910.GestureDetector.DataSources;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MF.Engineering.MF8910.GestureDetector.Mocking
{

    public class MockSkeleton : Skeleton
    {
        public MockSkeleton(): base()
        {
            TrackingState = SkeletonTrackingState.Tracked;
            Random random = new Random();
            SkeletonPoint p = new SkeletonPoint();
            p.X = (float)(random.Next(0, 4000000)) / 1000000;
            p.Y = (float)(random.Next(0, 4000000)) / 1000000;
            p.Z = (float)(random.Next(0, 4000000)) / 1000000;
            this.Position = p;
        }
    }

    public class MockSkeletonFrameReadyEventArgs : EventArgs
    {
        public Skeleton[] getSkeletons()
        {
            Random random = new Random();
            Skeleton[] result = new Skeleton[random.Next(0, 20)];
            for (int i = 0; i < result.Length; i++ )
            {
                result[i] = new MockSkeleton();
            }
            return result;
        }
    }

    public class MockKinectSensor: IDisposable
    {
        public event EventHandler<MockSkeletonFrameReadyEventArgs> SkeletonFrameReady;
        public KinectStatus Status { get { return KinectStatus.Connected; } }
        public bool IsRunning { get; set; }
        public void Start() 
        { 
            IsRunning = true;
            Thread t = new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    if (SkeletonFrameReady != null)
                    {
                        SkeletonFrameReady(this, new MockSkeletonFrameReadyEventArgs());
                    }
                    Thread.Sleep(10);
                }
            }));
            t.IsBackground = true;
            t.Start();
        }
        public void Stop() { IsRunning = false; }
        public void Dispose() { }
    }

}
