using System;
using System.Collections.Generic;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures;
using MF.Engineering.MF8910.GestureDetector.Gestures.Swipe;
using MF.Engineering.MF8910.GestureDetector.Tools;
using Microsoft.Kinect;
using System.Linq;

namespace MF.Engineering.MF8910.GestureDetector
{
    class Tester
    {
        private static JumpGestureChecker jgc;

        static void Main(string[] args)
        {
            Device d = new Device(); // Erstellen des des Devices
            d.NewPerson += NewPerson; // Registrieren auf neue Personen
            d.Start(); // Starten der Kinect

            Console.ReadLine();
        }

        static void NewPerson(object src, NewPersonEventArgs newPersonEventArgs )
        {
            jgc = new JumpGestureChecker(newPersonEventArgs.Person);
            jgc.Successful += delegate { Console.WriteLine("Jump"); };

        }
    }

    class JumpGestureChecker : GestureChecker
    {
        public JumpGestureChecker(Person p) : base(new List<Condition>() { new JumpCondition(p)}, 1000)
        {
        }
    }

    class JumpCondition : Condition
    {
        private Checker c;

        public JumpCondition(Person p) : base(p)
        {
            c = new Checker(p);
        }

        protected override void Check(object src, NewSkeletonEventArgs e)
        {
            if (c.GetAbsoluteMovement(JointType.HipCenter).Contains(Direction.Upward))
            {
                FireSucceeded(this, new JumpGestureEventArgs());
            }
            else
            {
                FireFailed(this, new FailedGestureEventArgs{Condition = this});
            }
        }
    }

    class JumpGestureEventArgs : GestureEventArgs{}
}
