using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;
using GestureEvents;
using Microsoft.Kinect;

namespace Gesture_Detector
{
    class Tester
    {
        static void Main(string[] args)
        {
            Device d = new Device();
            d.NewPerson += NewPerson;
            d.Start();
            int persons = 0;
            while (true)
            {
                if (persons != d.GetAll().Count)
                {
                    persons = d.GetAll().Count;
                    Console.WriteLine("new person count: " + persons);
                }
            }
        }

        static void NewPerson(object src, NewPersonEventArgs e)
        {
            Console.WriteLine(e.Person.ID);
            //e.Person.NewSkeleton += NewSeketon;
        }

        //static void NewSeketon(object src, NewSkeletonEventArg e)
        //{
        //    Console.WriteLine("new skeleton");
        //}
    }
}
