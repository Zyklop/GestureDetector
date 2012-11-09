using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Diagnostics;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures.Wave;

namespace MF.Engineering.MF8910.GestureDetector.DataSources
{
    public class Device
    {
        private static KinectSensor Dev;
        private Vector4 lastAcc;
        private List<Person> persons;
        private Dictionary<long, Person> cache; 

        public Device()
        {
            Dev = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            lastAcc = new Vector4();
            persons = new List<Person>();
            cache = new Dictionary<long, Person>();
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

                //Cache leeren
                long rem = -1;
                foreach (long l in cache.Keys)
                {
                    if (l < DateTime.Now.Ticks - 50000)
                    {
                        rem = l;
                    }
                }
                // remove Listeners on Person
                cache[rem].prepareToDie();
                // kill person
                cache.Remove(rem);

                /**
                 * Es gibt 3 verschiedene Möglichkeiten den aktuellen Status zu beschreiben:
                 * - Alle Personen hatten schon ein Skelett. Die Zuweisung muss neu erfolgen
                 * - Es gibt weniger Skelette als Personen. übrige Person muss gelöscht werden
                 * - Es gibt mehr Skelette als Personen. Eine neue Person muss erstellt werden
                 */
                Match bestMatch = new Match();

                if (skeletonList.Count == persons.Count) // jede Person bekommt ein neues Skelett
                {
                    foreach (Person p in persons) // für jede Person wird der beste Match gesucht
                    {
                        bestMatch.Value = double.MaxValue;
                        double v;
                        foreach (SmothendSkeleton s in skeletonList)
                        {
                            v = p.Match(s);
                            if (v < bestMatch.Value)
                            {
                                bestMatch.Value = v;
                                bestMatch.Person = p;
                                bestMatch.Skeleton = s;
                            }
                        }
                        bestMatch.Person.AddSkeleton(bestMatch.Skeleton); // weise neues Skelett zu
                    }
                }
                else if (skeletonList.Count < persons.Count) // eine Person ging aus dem Bild
                {
                    List<Person> personList = new List<Person>(); // Kopiere Personen für Matchingverfahren
                    personList.AddRange(persons);
                    foreach (SmothendSkeleton s in skeletonList) // für jedes Skelett wird der beste Match gesucht
                    {
                        bestMatch.Value = double.MaxValue;
                        double v;
                        foreach (Person p in personList) 
                        {
                            v = p.Match(s);
                            if (v < bestMatch.Value)
                            {
                                bestMatch.Value = v;
                                bestMatch.Person = p;
                                bestMatch.Skeleton = s;
                            }
                        }
                        bestMatch.Person.AddSkeleton(bestMatch.Skeleton); // weise neues Skelett zu
                        personList.Remove(bestMatch.Person);
                        cache.Add(System.DateTime.Now.Ticks, bestMatch.Person);
                    }
                    // Lösche übriggebliebene Personen, da sie kein Skelett mehr haben
                    foreach (Person p in personList) 
                    {
                        persons.Remove(p);
                    }
                } 
                else // eine Person kam ins Bild
                {
                    List<Person> personList = new List<Person>(); // Kopiere Personen für Matchingverfahren
                    personList.AddRange(persons);
                    foreach (Person p in personList) // für jede Person wird der beste Match gesucht
                    {
                        bestMatch.Value = double.MaxValue;
                        double v;
                        foreach (SmothendSkeleton s in skeletonList)
                        {
                            v = p.Match(s);
                            if (v < bestMatch.Value)
                            {
                                bestMatch.Value = v;
                                bestMatch.Person = p;
                                bestMatch.Skeleton = s;
                            }
                        }
                        bestMatch.Person.AddSkeleton(bestMatch.Skeleton); // weise neues Skelett zu
                        skeletonList.Remove(bestMatch.Skeleton);
                    }
                    //Match to Cache
                    List<SmothendSkeleton> skeletonsToRemove = new List<SmothendSkeleton>();
                    foreach (SmothendSkeleton s in skeletonList)
                    {
                        bestMatch.Value = double.MaxValue;
                        double v;
                        foreach (long l in cache.Keys)
                        {
                            Person p = cache[l];
                            v = p.Match(s);
                            if (v < bestMatch.Value)
                            {
                                bestMatch.Value = v;
                                bestMatch.Person = p;
                                bestMatch.Skeleton = s;
                                bestMatch.Key = l;
                            }
                        }
                        if (bestMatch.Value < 0.5) // übereinstimmung gültig
                        {
                            persons.Add(bestMatch.Person);
                            registerWave(bestMatch.Person);
                            bestMatch.Person.AddSkeleton(bestMatch.Skeleton);
                            cache.Remove(bestMatch.Key);
                            skeletonsToRemove.Add(bestMatch.Skeleton);
                        }
                    }
                    foreach (SmothendSkeleton ske in skeletonsToRemove)
                    {
                        skeletonList.Remove(ske);
                    }
                    skeletonsToRemove.Clear();
                    // erstelle für übrige Skelette jeweils Personen
                    foreach (SmothendSkeleton s in skeletonList)
                    {
                        Person p = new Person(this);
                        p.AddSkeleton(s);
                        persons.Add(p);
                        registerWave(p);
                        if (NewPerson != null)
                        {
                            NewPerson(this, new NewPersonEventArgs(p));
                        }
                    }
                }
                skeFrm.Dispose();
            }
            lastAcc = Dev.AccelerometerGetCurrentReading();
        }

        private void registerWave(Person person)
        {
            person.OnWave += personWaved;

        }

        private void personWaved(object sender, GestureEventArgs e)
        {
            Person p = ((Person)sender);
            if (!p.Active)
            {
                p.Active = true;
                PersonActive(this,new ActivePersonEventArgs(p));
                p.OnWave -= personWaved;
            }
        }

        private class Match
        {
            public double Value {get; set; }
            public Person Person {get; set; }
            public SmothendSkeleton Skeleton {get; set; }
            public long Key { get; set; }
        };

        #region Events

        public event EventHandler<ActivePersonEventArgs> PersonActive;
        //public delegate void ActivePersonHandler<TEventArgs> (object source, TEventArgs e) where TEventArgs:EventArgs;

        public event EventHandler<NewPersonEventArgs> NewPerson;
        //public delegate void PersonHandler<TEventArgs>(object source, TEventArgs e) where TEventArgs : EventArgs;

        public event EventHandler<AccelerationEventArgs> Accelerated;

        #endregion
    }
}
