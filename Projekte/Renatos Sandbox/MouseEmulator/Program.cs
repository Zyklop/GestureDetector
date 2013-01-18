using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputEmulation;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;

namespace MouseEmulator
{
    class Program
    {
        private static Device _d;
        private static Person _active;
        private static JoystickGestureChecker jgc;
        private static double _lastZ;
        private static bool leftdown;
        private static bool rightdown;

        static void Main(string[] args)
        {
            _d = new Device();
            _d.NewPerson += delegate
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
            jgc = new JoystickGestureChecker(_active, false);
            jgc.Successful += Movement;
        }

        private static void Movement(object sender, GestureEventArgs e)
        {
            var args = (JoystickGestureEventArgs) e;
            //Console.WriteLine(String.Format("X: {0} Y: {1} DistToShoulderZ: {2}", args.X, args.Y, args.DistToShoulderZ));
            if (!(Math.Abs(args.DistToShoulderZ - _lastZ) > 0.01))
            {
                Mouse.MoveMouseRelative((int) Math.Round(args.X*100), (int) Math.Round(args.Y*100));
            }
            if (args.DistToShoulderZ < 0.35)
            {
                if (leftdown)
                {
                    Mouse.ClickEvent(true, true);
                    leftdown = false;
                }
                Console.WriteLine("Rightclick");
                Mouse.ClickEvent(false,false);
                rightdown = true;
            } else if (args.DistToShoulderZ > 0.5)
            {
                Console.WriteLine("leftclick");
                if (rightdown)
                {
                    Mouse.ClickEvent(false, true);
                    rightdown = false;
                }
                Mouse.ClickEvent(true, false);
                leftdown = true;
            }
            else
            {
                if (rightdown)
                {
                    Mouse.ClickEvent(false, true);
                    rightdown = false;
                }
                if (leftdown)
                {
                    Mouse.ClickEvent(true, true);
                    leftdown = false;
                }
            }
            _lastZ = args.DistToShoulderZ;
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
            if (Equals(e.Person, _active))
            {
                RemoveActive();
            }
            Console.WriteLine("User disapeared");
        }

        private static void RemoveActive()
        {
            _active = null;
            jgc.Successful -= Movement;
            jgc = null;
        }
    }
}
