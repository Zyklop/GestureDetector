using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures.Zoom;
using System.Diagnostics;

namespace MF.Engineering.MF8910.GestureDetector
{
    class Tester
    {
        static void Main(string[] args)
        {
            Device d = new Device();
            d.NewPerson += NewPerson;
            d.Start();

            Console.ReadLine();
        }

        static void NewPerson(object src, NewPersonEventArgs e)
        {
            e.Person.OnWave += delegate { Console.Write("!"); };
            Console.WriteLine(e.Person.ID);
        }
    }
}
