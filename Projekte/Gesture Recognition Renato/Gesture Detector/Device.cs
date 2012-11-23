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
        private Vector4 lastAcceleration; // last accelerometer readings
        private List<Person> persons; //active persons
        private Dictionary<long, Person> cache; //Persons from the last 5 seconds

        public Device()
        {
            // get a Kinect
            Dev = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            initialize();
        }

        private void initialize()
        {
            lastAcceleration = new Vector4();
            persons = new List<Person>();
            cache = new Dictionary<long, Person>();
            Dev.SkeletonStream.Enable(); // to get skeletons
            Dev.SkeletonFrameReady += NewSkeletons; // register on new skeletons
        }

        public Device(string uniqueId) // get a spacified Kinect by its ID
        {
            foreach (KinectSensor ks in KinectSensor.KinectSensors)
            {
                if (ks.UniqueKinectId == uniqueId)
                {
                    Dev = ks;
                    initialize();
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

        public Person GetActive() // get all
        {
            return persons.Find(x => x.Active == true);
        }

        public List<Person> GetAll()
        {
            return persons;
        }

        // TODO wie wärs mit einem griffigeren Namen?
        protected void NewSkeletons(object source, SkeletonFrameReadyEventArgs e)
        {
            double diff = getAccelerationDiff();
            if ((diff > 0.1 || diff < -0.1) && Accelerated != null) 
            {
                //Device not stable
                Accelerated(this, new AccelerationEventArgs(diff));
            }
            else
            {
                // Device stable
                SkeletonFrame skeFrm = e.OpenSkeletonFrame();
                //TODO increase Performante to remove this
                if (skeFrm != null)
                {
                    //get skeletons
                    Skeleton[] skeletons = new Skeleton[skeFrm.SkeletonArrayLength];
                    skeFrm.CopySkeletonDataTo(skeletons);
                    // use only active skeletons
                    List<SmothendSkeleton> skeletonList = new List<SmothendSkeleton>();
                    //findall not possible
                    foreach (Skeleton ske in skeletons)
                    {
                        if (ske.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            skeletonList.Add(new SmothendSkeleton(ske));
                        }
                    }

                    //remove old person from cache
                    long rem = -1;
                    foreach (long l in cache.Keys)
                    {
                        if (l < DateTime.Now.Ticks - 50000)
                        {
                            rem = l;
                        }
                    }
                    if (rem != -1)
                    {
                        // remove Listeners on Person
                        cache[rem].prepareToDie();
                        // kill person
                        cache.Remove(rem);
                    }


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
                            cache.Add(System.DateTime.Now.Ticks, bestMatch.Person);//add person to cache
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
                            if (bestMatch.Value < 0.5) // match valid
                            {
                                //set person active again
                                persons.Add(bestMatch.Person);
                                registerWave(bestMatch.Person);
                                bestMatch.Person.AddSkeleton(bestMatch.Skeleton);//give the new Skeleton
                                cache.Remove(bestMatch.Key);
                                skeletonsToRemove.Add(bestMatch.Skeleton);
                            }
                        }
                        foreach (SmothendSkeleton ske in skeletonsToRemove)
                        {
                            skeletonList.Remove(ske);
                        }
                        //new person for unmatched skeletons
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
            }
        }
        // set person active after waving
        private void registerWave(Person person)
        {
            person.OnWave += personWaved;

        }


        private double getAccelerationDiff()
        {
            double diff = 0; // Difference between last accelerometer readings and actual readings
            diff += (Dev.AccelerometerGetCurrentReading().W - lastAcceleration.W);
            diff += (Dev.AccelerometerGetCurrentReading().X - lastAcceleration.X);
            diff += (Dev.AccelerometerGetCurrentReading().Y - lastAcceleration.Y);
            diff += (Dev.AccelerometerGetCurrentReading().Z - lastAcceleration.Z);
            lastAcceleration = Dev.AccelerometerGetCurrentReading();
            return diff;
        }

        private void personWaved(object sender, GestureEventArgs e)
        {
            Person p = ((Person)sender);
            // set active if ther isn't another active person
            if (!p.Active && persons.Find(x => x.Active==true)==null)
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
