using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;
using GestureEvents;

namespace Gesture_Detector
{
    class Tester
    {
        static void Main(string[] args)
        {
            Device d = new Device();
            d.NewPerson += NewPerson;
            d.Start();
            int pers = 0;
            while (true)
            {
                if (pers != d.GetAll().Count)
                {
                    Console.WriteLine(d.GetAll().Count);
                    pers = d.GetAll().Count;
                }
            }

        }

        static void NewPerson(object src, NewPersonEventArgs e)
        {
            Console.WriteLine(e.Person.ID);
            e.Person.OnWave += delegate(object o, EventArgs ev) { Console.WriteLine("gewinkt"); };
        }

    }
}
