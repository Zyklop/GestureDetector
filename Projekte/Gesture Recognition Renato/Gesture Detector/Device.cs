using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using GestureEvents;
using Gesture_Detector;
using System.Diagnostics;

namespace DataSources
{
    public class Device
    {
        private static KinectSensor Dev;
        private Vector4 lastAcc;
        private List<Person> persons;

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
            double diff = 0;
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

                // Berücksichtige nur getrackte Skelette
                List<SmothendSkeleton> skeletonList = new List<SmothendSkeleton>();
                foreach (Skeleton ske in skeletons)
                {
                    if (ske.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        skeletonList.Add(new SmothendSkeleton(ske));
                    }
                }
                Debug.WriteLine(persons.Count + " Personen " + skeletonList.Count + " skeletons");
                /**
                 * Matchmatrix - 7 Skelette werden mit je 7 Personen gematcht
                 * 
                 *      | P1 | P2 | P3 | ..
                 *   S1 |  1 | 12 |  2 | ..
                 *   S2 |  5 | 15 |  7 | ..
                 *    : |  : |  : |  : | ..
                 *
                 * Ein tiefes Matching bedeutet dass ein neues Skelett und ein 
                 * bestehendes einer Person näher zusammenliegen als bei einem
                 * hohen
                 */
                //double[,] matches = new double[7,7]; // da 4-fach verkettete Listen blöd sind, nehmen wir einen Array
                //for (int i = 0; i < persons.Count; i++) // für alle Personen
                //{
                //    for (int j = 0; j < skeletonList.Count; j++) // für alle Skelette
                //    {
                //        matches[i,j] = persons[i].Match(skeletonList[j]);
                //    }
                //}

                /**
                 * Es gibt 3 verschiedene Möglichkeiten den aktuellen Status zu beschreiben:
                 * - Alle Personen hatten schon ein Skelett. Die Zuweisung muss neu erfolgen
                 * - Es gibt weniger Skelette als Personen. übrige Person muss gelöscht werden
                 * - Es gibt mehr Skelette als Personen. Eine neue Person muss erstellt werden
                 */
                //Dictionary<Person, SmothendSkeleton> bestMatches = new Dictionary<Person, SmothendSkeleton>();
                if (skeletonList.Count == persons.Count) // jede Person bekommt ein neues Skelett
                {
                    foreach (Person pers in persons)
                    {
                        double min = Double.MaxValue;
                        int matchindex = 0;
                        for (int i = 0; i < skeletonList.Count; i++)
                        {
                            if (skeletonList[i] != null)
                            {
                                double match = pers.Match(skeletonList[i]);
                                if (match < min)
                                {
                                    matchindex = i;
                                    min = match;
                                }
                            }
                        pers.AddSkeleton(skeletonList[matchindex]);
                        skeletonList[matchindex] = null;
                        }
                    }
                }
                else if (skeletonList.Count < persons.Count) // eine Person ging aus dem Bild (persons > skeletons)
                {
                    bool[] persused = new bool[persons.Count];
                    for (int i = 0; i < persused.Length; i++)
                    {
                        persused[i] = false;
                    }
                    foreach (SmothendSkeleton skel in skeletonList)
                    {
                        double min = Double.MaxValue;
                        int matchindex = 0;
                        for (int i = 0; i < persons.Count; i++)
                        {
                            if (persused[i] == false)
                            {
                                double match = persons[i].Match(skel);
                                if (match < min)
                                {
                                    matchindex = i;
                                    min = match;
                                }
                            }
                            persons[matchindex].AddSkeleton(skel);
                            persused[matchindex] = false;
                        }
                    }
                    for (int i = 0; i < persused.Length; i++)
                    {
                        if (!persused[i])
                        {
                            if (i < persons.Count)
                            {
                                persons.RemoveAt(i);
                            }
                        }
                    }
                }
                else // eine Person kam ins Bild (skeletons > persons)
                {
                    foreach (Person pers in persons)
                    {
                        double min = Double.MaxValue;
                        int matchindex = 0;
                        for (int i = 0; i < skeletonList.Count; i++)
                        {
                            if (skeletonList[i] != null)
                            {
                                double match = pers.Match(skeletonList[i]);
                                if (match < min)
                                {
                                    matchindex = i;
                                    min = match;
                                }
                            }
                            pers.AddSkeleton(skeletonList[matchindex]);
                            skeletonList[matchindex] = null;
                        }
                    }
                    foreach (SmothendSkeleton ss in skeletonList)
                    {
                        if (ss != null)
                        {
                            Person newPers = new Person(this);
                            newPers.AddSkeleton(ss);
                            //newPers.AddSkeleton(skeletonList.Find(x => x != null));
                            persons.Add(newPers);
                            NewPerson(this, new NewPersonEventArgs(newPers));
                        }
                    }
                }
                skeFrm.Dispose();
            }
            lastAcc = Dev.AccelerometerGetCurrentReading();
        }

        /**
         * Es wird das Minimum in einem Array gesucht. Die ganze Zeile und die 
         * ganze Spalte des Fundortes wird gelöscht (bzw. auf unendlich gesetzt).
         * 
         *      | P1 | P2 | P3 | ..
         *   S1 |  8 |  ∞ | 12 | ..
         *   S2 | 15 |  ∞ |  7 | ..
         *   S3 |  ∞ |  ∞ |  ∞ | ..
         *   S4 | 75 |  ∞ | 47 | ..
         *    : |  : |  : |  : | ..
         * 
         * Bsp: Es wurde [P2, S3] als Minimum gefunden
         */
        //private Tuple<int, int> iterateMatches(ref double[,] matches)
        //{
        //    // finde Minimum
        //    int matchI = -1;
        //    int matchJ = -1;
        //    int minMatch = int.MaxValue;
        //    for (int i = 0; i < matches.GetLength(0); i++) // für alle Personen
        //    {
        //        for (int j = 0; j < matches.GetLength(1); j++) // für alle Skelette
        //        {
        //            if (matches[i, j] < minMatch)
        //            {
        //                matchI = i;
        //                matchJ = j;
        //            }
        //        }
        //    }

        //    // speichere Minimum
        //    Tuple<int, int> found = new Tuple<int, int>(matchI, matchJ);

        //    // überschreibe Zeile und Spalte des Fundortes
        //    for (int i = 0; i < matches.GetLength(0); i++) // überschreibe Zeile des gefundenen Minimums
        //    {
        //        matches[i, matchJ] = int.MaxValue;
        //    }
        //    for (int j = 0; j < matches.GetLength(1); j++) // überschreibe Spalte des gefundenen Minimums
        //    {
        //        matches[matchI, j] = int.MaxValue;
        //    }

        //    return found;
        //}

        #region Events

        public event EventHandler<ActivePersonEventArgs> PersonActive;
        //public delegate void ActivePersonHandler<TEventArgs> (object source, TEventArgs e) where TEventArgs:EventArgs;

        public event EventHandler<NewPersonEventArgs> NewPerson;
        //public delegate void PersonHandler<TEventArgs>(object source, TEventArgs e) where TEventArgs : EventArgs;

        public event EventHandler<AccelerationEventArgs> Accelerated;

        #endregion
    }
}
