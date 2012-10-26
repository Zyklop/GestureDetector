using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using GestureEvents;
using Gesture_Detector;

namespace DataSources
{
    public class Device
    {
        private static KinectSensor Dev;
        private Vector4 lastAcc;
        private List<Person> persons;
        private double[,] matches = new double[7,7];

        public Device()
        {
            Dev = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            lastAcc = new Vector4();
            persons = new List<Person>();
            Dev.SkeletonStream.Enable();
            Dev.SkeletonFrameReady += NewSkeletons;
        }

        public Device(string uniqueId)
        {
            foreach (KinectSensor ks in KinectSensor.KinectSensors)
            {
                if (ks.UniqueKinectId == uniqueId)
                {
                    Dev = ks;
                }
            }
        }

        public KinectStatus Status { get { return Dev.Status; } }

        public bool Start()
        {
            if (!Dev.IsRunning)
            {
             Dev.Start();
            }
            return Dev.IsRunning;
        }

        public bool Stop()
        {
            if (Dev.IsRunning)
            {
                Dev.Stop();
            }
            return Dev.IsRunning;
        }

        public void Dispose()
        {
            Dev.Dispose();
        }

        public Person GetActive()
        {
            return null;
        }

        public List<Person> GetAll()
        {
            return persons;
        }

        // TODO wie wärs mit einem griffigeren Namen?
        void NewSkeletons(object source, SkeletonFrameReadyEventArgs e)
        {
            double diff=0;
			diff += (Dev.AccelerometerGetCurrentReading().W - lastAcc.W);
            diff += (Dev.AccelerometerGetCurrentReading().X - lastAcc.X);
            diff += (Dev.AccelerometerGetCurrentReading().Y - lastAcc.Y);
            diff += (Dev.AccelerometerGetCurrentReading().Z - lastAcc.Z);
            if ((diff > 0.1 || diff < -0.1) && Accelerated != null)
            {
                Accelerated(this, new AccelerationEventArgs(diff));
            }
            else
            {
                SkeletonFrame skeFrm = e.OpenSkeletonFrame();
                Skeleton[] skeletons = new Skeleton[skeFrm.SkeletonArrayLength];
                skeFrm.CopySkeletonDataTo(skeletons);
                List<SmothendSkeleton> skelList = new List<SmothendSkeleton>();
                //List<Match> matches = new List<Match>();
                foreach (Skeleton ske in skeletons)
                {
                    if (ske.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        skelList.Add(new SmothendSkeleton(ske));
                    }
                }

                for (int i = 0; i < matches.GetLength(0); i++ )
                {
                    for(int j = 0; j < matches.GetLength(1); i++) {
                        persons[i].CompareTo(skeletons[j]);
                    }
                }

                //if (active > persons.Count)
                //{
                //    Person p = new Person(this);
                //    persons.Add(p);
                //    //matches.Add(new Match(matches.Min().Skeleton, p, 0.0));
                //    NewPerson(this,new NewPersonEventArgs(p));
                //}
                List<Person> ptemp = new List<Person>();
                ptemp.AddRange(persons);
                //for (int i = 0; i < skelList.Count;i++)
                //{
                //    Person p=FindBestMatch(skelList[i], ptemp);
                //    p.AddSkeleton(skelList[i]);
                //    skelList[i] = null;
                    
                //}
                //else if (active < persons.Count)
                //{
                //    persons.Remove(matches.Max().Person);
                //}
                //foreach (Skeleton ske in skeletons)
                //{
                //    if (ske.TrackingState == SkeletonTrackingState.Tracked)
                //    {
                //        SmothendSkeleton smooth = new SmothendSkeleton(ske);
                //        matches.Add(FindBestMatch(smooth));
                //    }
                //}
                // GestureChecker wave; // eher bei Person
            }
            lastAcc = Dev.AccelerometerGetCurrentReading();
        }

        private Person FindBestMatch(SmothendSkeleton s, List<Person> plist)
        {
            double[] diffs = new double[plist.Count];
            double min = Double.MaxValue;
            int minpos = 0;
            for (int i = 0; i < plist.Count; i++)
            {
                if (plist[i] == null)
                {
                    diffs[i] = 0;
                    diffs[i] += s.GetPosition(JointType.HipCenter).X - plist[i].CurrentSkeleton.GetPosition(JointType.HipCenter).X;
                    diffs[i] += s.GetPosition(JointType.HipCenter).Y - plist[i].CurrentSkeleton.GetPosition(JointType.HipCenter).Y;
                    diffs[i] += s.GetPosition(JointType.HipCenter).Z - plist[i].CurrentSkeleton.GetPosition(JointType.HipCenter).Z;
                    if (diffs[i] < min)
                    {
                        min = diffs[i];
                        minpos = i;
                    }
                }
            }
            return plist[minpos];
        }

        public event EventHandler<ActivePersonEventArgs> PersonActive;
        //public delegate void ActivePersonHandler<TEventArgs> (object source, TEventArgs e) where TEventArgs:EventArgs;

        public event EventHandler<NewPersonEventArgs> NewPerson;
        //public delegate void PersonHandler<TEventArgs>(object source, TEventArgs e) where TEventArgs : EventArgs;

        public event EventHandler<AccelerationEventArgs> Accelerated;
    }

    public class Match : IComparable
    {
        private SmothendSkeleton skel;
        private Person pers;
        private double delta;


        public Match(SmothendSkeleton ske, Person p, double diff)
        {
            skel = ske;
            pers = p;
            delta = diff;
        }

        public SmothendSkeleton Skeleton { get { return skel; } }
        public Person Person { get { return pers; } }
        public double Delta { get { return delta; } }

        public int CompareTo(object obj)
        {
            return Delta.CompareTo(((Match)obj).Delta);
        }
    }
}
