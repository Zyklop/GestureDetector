using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;

namespace Gesture_Detection_Api
{
    class Program
    {
        static void p1_WaveSucceeded(object sender, EventArgs e)
        {
            Console.WriteLine("waved successfully.");
        }

        static void Main(string[] args)
        {
            Device d = new Device();
            Person p1 = new Person(d);
            p1.OnWave += new EventHandler(p1_WaveSucceeded);
        }
    }
}
