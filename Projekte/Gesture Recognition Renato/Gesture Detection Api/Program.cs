using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gesture_Detection_Api
{
    class Program
    {
        static void p1_GestureSucceeded(object sender, EventArgs e)
        {
            Console.WriteLine("lol");
        }

        static void Main(string[] args)
        {
            Device d = new Device();
            Person p1 = new Person(d);
            p1.OnWave += new Gesture_Detection_Api.Person.GestureSucceededEventHandler(p1_GestureSucceeded);
        }
    }
}
