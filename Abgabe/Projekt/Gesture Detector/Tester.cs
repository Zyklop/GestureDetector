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
    /// <summary>
    /// Demo Code 
    /// </summary>
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
            jgc = new JumpGestureChecker(newPersonEventArgs.Person); // Anlegen des Eigenen GestureCheckers 
            jgc.Successful += delegate { Console.WriteLine("Jump"); }; // Registrieren auf dessen Event

        }
    }

    class JumpGestureChecker : GestureChecker // Klasse implementiert GestureChecker
    {
        public JumpGestureChecker(Person p) // übergeben der zu überwachenden Person
            : base(new List<Condition>
                {
                    new JumpCondition(p) // Anlegen einens GestureCheckers mit einer JumpCondition
                }, 1000) // timerout ist hier nicht von Belang
        {
        }
    }

    class JumpCondition : Condition // JumpCondition prüft, ob gesprungen wurde
    {
        private Checker c; // Checker für die Berechnungen

        public JumpCondition(Person p) : base(p)
        {
            c = new Checker(p);
        }

        protected override void Check(object src, NewSkeletonEventArgs e) // überprüfung bei jedem neuen Skelett
        {
            if (c.GetAbsoluteMovement(JointType.HipCenter) // Bewegung der Hüfte
                .Contains(Direction.Upward)) // nach oben?
            {
                FireSucceeded(this, new JumpGestureEventArgs()); // Condition erfolgreich
            }
            else
            {
                FireFailed(this, new FailedGestureEventArgs{Condition = this}); // nicht erfolgreich
            }
        }
    }

    class JumpGestureEventArgs : GestureEventArgs{} // Args für optionale Parameter
}
