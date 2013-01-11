using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Tools;

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
            double dist = ((ThrustGestureEventArgs) e).DistanceToKnee;
            if (dist > -0.12)
            {
                Console.Write("Forward");
                if (downdown)
                {
                    KeyBoard3.KeyUp(Keys.Down);
                    downdown = false;
                }
                if (!updown)
                {
                    KeyBoard3.KeyDown(Keys.Up);
                    updown = true;
                }
                //Keyboard.SendKeyAsInput(Keys.Up,30);
                if (dist > 0.0)
                {
                    KeyBoard3.KeyDown(Keys.Alt);
                    Console.Write(" with nitro! ");
                }
                else
                {
                    KeyBoard3.KeyUp(Keys.Alt);
                }
            }
            else if (dist > -0.24)
            {
                Console.Write("Neutral ");
                if (downdown)
                {
                    KeyBoard3.KeyUp(Keys.Down);
                    downdown = false;
                }
                if (updown)
                {
                    KeyBoard3.KeyUp(Keys.Up);
                    updown = false;
                }
            }
            else
            {
                Console.Write("Break ");
                if (updown)
                {
                    KeyBoard3.KeyUp(Keys.Up);
                    updown = false;
                }
                if (!downdown)
                {
                    KeyBoard3.KeyDown(Keys.Down);
                    downdown = true;
                }
                //Keyboard.SendKeyAsInput(Keys.Down, 30);
            }
        }

        private static void Steered(object sender, GestureEventArgs e)
        {
            Direction direction = ((SteeringGestureEventArgs) e).Direction;
            Console.WriteLine(" steering " + direction);
            if (direction == Direction.Left)
            {
                if (rightdown)
                {
                    KeyBoard3.KeyUp(Keys.Right);
                    rightdown = false;
                }
                if (!leftdown)
                {
                    KeyBoard3.KeyDown(Keys.Left);
                    leftdown = true;
                }
                //Keyboard.SendKeyAsInput(Keys.Left,30);
            }
            else if (direction == Direction.Right)
            {
                if (leftdown)
                {
                    KeyBoard3.KeyUp(Keys.Left);
                    leftdown = false;
                }
                if (!rightdown)
                {
                    KeyBoard3.KeyDown(Keys.Right);
                    rightdown = true;
                }
                //Keyboard.SendKeyAsInput(Keys.Right);
            }
            else if (direction == Direction.None)
            {
                if (leftdown)
                {
                    KeyBoard3.KeyUp(Keys.Left);
                    leftdown = false;
                }
                if (rightdown)
                {
                    KeyBoard3.KeyUp(Keys.Right);
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
        }
    }
}
