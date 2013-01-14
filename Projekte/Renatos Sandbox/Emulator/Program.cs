using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Tools;
using InputEmulation;

namespace Emulator
{
    class Program
    {
        private static Device _d;
        private static Person _active;
        private static SteeringGestureChecker sgc;
        private static ThrustGestureChecker tgc;
        private static bool leftdown;
        private static bool rightdown;
        private static bool downdown;
        private static bool updown;
        private static bool altdown;

        static void Main(string[] args)
        {
            _d = new Device();
            _d.NewPerson += delegate(object sender, NewPersonEventArgs eventArgs)
                {
                    Console.WriteLine("new User visible");
                };
            _d.PersonActive += ActivePerson;
            _d.PersonLost += Dispose;
            _d.Start();
            while (true)
            {
                System.Threading.Thread.Sleep(5);
            }
        }

        private static void ActivePerson(object sender, ActivePersonEventArgs e)
        {
            _active = e.Person;
            Console.WriteLine("User Active");
            _active.OnWave += ActWaved;
            sgc = new SteeringGestureChecker(e.Person);
            tgc = new ThrustGestureChecker(e.Person);
            sgc.Successful += Steered;
            tgc.Successful += Thrusted;
        }

        private static void Thrusted(object sender, GestureEventArgs e)
        {
            double dist = ((ThrustGestureEventArgs) e).DistanceToShoulder;
            //Console.Write(dist + " ");
            if (dist > 0.45)
            {
                Console.Write("Forward");
                if (downdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_DOWN);
                    downdown = false;
                }
                if (!updown)
                {
                    DirectInput.KeyDown(DirectInput.VK_UP);
                    updown = true;
                }
                //VirtualKeys.SendKeyAsInput(Keys.Up,30);
                if (dist > 0.6)
                {
                    //VirtualKeys.SendKeyAsInput(Keys.Alt,30);
                    if (!altdown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_LALT);
                        altdown = true;
                    }
                    Console.Write(" with nitro! ");
                }
                else
                {
                    if (altdown)
                    {
                        DirectInput.KeyUp(DirectInput.VK_LALT);
                        altdown = false;
                    }
                }
            }
            else if (dist > 0.3)
            {
                Console.Write("Neutral ");
                if (downdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_DOWN);
                    downdown = false;
                }
                if (updown)
                {
                    DirectInput.KeyUp(DirectInput.VK_UP);
                    updown = false;
                }
            }
            else
            {
                Console.Write("Break ");
                if (updown)
                {
                    DirectInput.KeyUp(DirectInput.VK_UP);
                    updown = false;
                }
                if (!downdown)
                {
                    DirectInput.KeyDown(DirectInput.VK_DOWN);
                    downdown = true;
                }
                //VirtualKeys.SendKeyAsInput(Keys.Down, 30);
            }
        }

        private static void Steered(object sender, GestureEventArgs e)
        {
            var direction = ((SteeringGestureEventArgs) e).Direction;
            Console.WriteLine(" steering " + direction);
            if (direction < 0)
            {
                if (rightdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_RIGHT);
                    rightdown = false;
                }
                if (direction == -2)
                {
                    if (!leftdown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_LEFT);
                        leftdown = true;
                    }
                }
                else
                {
                    if (!leftdown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_LEFT);
                        leftdown = true;
                    }
                    else
                    {
                        DirectInput.KeyUp(DirectInput.VK_LEFT);
                        leftdown = false;
                    }
                }
                //VirtualKeys.SendKeyAsInput(Keys.Left,30);
            }
            else if (direction > 0)
            {
                if (leftdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_LEFT);
                    leftdown = false;
                }
                if (direction == 2)
                {
                    if (!rightdown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_RIGHT);
                        rightdown = true;
                    }
                }
                else
                {
                    if (!rightdown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_RIGHT);
                        rightdown = true;
                    }
                    else
                    {
                        DirectInput.KeyUp(DirectInput.VK_RIGHT);
                        rightdown = false;
                    }
                }
                //VirtualKeys.SendKeyAsInput(Keys.Right);
            }
            else
            {
                if (leftdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_LEFT);
                    leftdown = false;
                }
                if (rightdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_RIGHT);
                    rightdown = false;
                }
            }
        }

        private static void ActWaved(object sender, GestureEventArgs e)
        {
            Console.WriteLine("User inactive");
            _active.OnWave -= ActWaved;
            _active.Active = false;
            _active = null;
        }

        private static void Dispose(object sender, PersonDisposedEventArgs e)
        {
            if (e.Person == _active)
            {
                RemoveActive();
            }
            Console.WriteLine("User disapeared");
        }

        private static void RemoveActive()
        {
            _active = null;
            sgc.Successful -= Steered;
            tgc.Successful -= Thrusted;
        }
    }
}
