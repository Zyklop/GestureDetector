using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Microsoft.Kinect;

namespace KinectConsole
{
    class Program
    {
        private static KinectSensor Dev = KinectSensor.KinectSensors.First();


        static void Main(string[] args)
        {
            JointType[] points = { JointType.ShoulderCenter, JointType.Head};
            String filename ="C:\\Temp\\Winken.csv";
            int pretimer = 100;
            int timer = 300;

            GetKinect();
            Dev.Start();
            Console.WriteLine(Dev.IsRunning);
            Dev.ElevationAngle = 5;
            Dev.SkeletonStream.Enable();
            StreamWriter writeFile = new StreamWriter(new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write));
            foreach (JointType jt in points)
            {
                writeFile.Write(jt.ToString() + ".X;" + jt.ToString() + ".Y;" + jt.ToString() + ".Z;");
            }
            writeFile.WriteLine();
            for (int i = pretimer; i > 0; i--)
            {
                SkeletonFrame Frm = Dev.SkeletonStream.OpenNextFrame(100);
                if (!(Frm == null))
                {
                    Skeleton[] skeletons = new Skeleton[6];
                    Frm.CopySkeletonDataTo(skeletons);
                    Skeleton ske = skeletons.FirstOrDefault(x => x.TrackingState == SkeletonTrackingState.Tracked);
                    if (!(ske == null))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Starting in " + i);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Starting in " + i);
                    }
                    System.Threading.Thread.Sleep(33);
                    Frm.Dispose();
                }
            }
            for (int i = timer; i > 0; i--)
            {
                SkeletonFrame Frm = Dev.SkeletonStream.OpenNextFrame(50);
                if (! (Frm == null))
                {
                Skeleton[] skeletons = new Skeleton[6];
                    Frm.CopySkeletonDataTo(skeletons);
                    Skeleton ske = skeletons.FirstOrDefault(x => x.TrackingState == SkeletonTrackingState.Tracked);
                    if (!(ske == null))
                    {
                        foreach (JointType jt in points)
                        {
                            writeFile.Write(ske.Joints[jt].Position.X + ";" + ske.Joints[jt].Position.Y + ";" +
                                ske.Joints[jt].Position.Z + ";");
                        }
                        writeFile.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Skeleton recogniced");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Not recogniced!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    }
                    System.Threading.Thread.Sleep(33);
                    Frm.Dispose();
                }
            }
            writeFile.Close();
            Console.WriteLine("File saved to: " + filename);
            Console.Read();
        }

        public static void GetKinect()
        {
            Dev = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }

    }
}
