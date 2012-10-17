using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSources
{
    class Person
    {
        private bool active;

        public Person()
        {

        }

        public StaticSmothendSkeleton getStaticSkeleton()
        {
            return null;
        }

        public MovingSmothendSkeleton getMovingSkeleton()
        {
            return null;
        }

        public StaticSmothendSkeleton getLastStaticSkeleton(int frames)
        {
            return null;
        }

        public MovingSmothendSkeleton getLastMovingSkeleton(int frames)
        {
            return null;
        }

        public bool Active { get; set; }

        public event EventHandler NewSkeleton;
    }
}
