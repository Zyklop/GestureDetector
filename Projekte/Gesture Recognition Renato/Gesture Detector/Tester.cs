using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures.Zoom;
using System.Diagnostics;
using MF.Engineering.MF8910.GestureDetector.Mocking;

namespace MF.Engineering.MF8910.GestureDetector
{
    class Tester
    {
        static int personCount = 0;

        static void Main(string[] args)
        {
            Device d = new Device();
            d.NewPerson += NewPerson;
            d.Start();

            Console.ReadLine();
        }

        static void NewPerson(object src, NewPersonEventArgs e)
        {
            Console.WriteLine(++personCount);
            e.Person.OnSwipe += delegate
            {
                Random random = new Random();
                byte[] list = new byte[10000000];
                for (int i = 0; i < list.Length; i++ )
                {
                    list[i] = (byte)random.Next(100);
                }
                Console.WriteLine(list[50000]);
            };
            e.Person.OnWave += delegate
            {
                Random random = new Random();
                byte[] list = new byte[10000000];
                for (int i = 0; i < list.Length; i++)
                {
                    list[i] = (byte)random.Next(100);
                }
                Console.WriteLine(list[50000]);
            };
            e.Person.OnZoom += delegate
            {
                Random random = new Random();
                byte[] list = new byte[10000000];
                for (int i = 0; i < list.Length; i++)
                {
                    list[i] = (byte)random.Next(100);
                }
                Console.WriteLine(list[50000]);
            };
        }
    }
}
