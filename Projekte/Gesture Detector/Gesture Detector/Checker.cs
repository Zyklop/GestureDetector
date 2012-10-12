using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSources;
using Microsoft.Kinect;

namespace Conditions
{
    public enum Direction
    {
        forward, upward, downward, left, right, backward, none
    }

    class Checker
    {
        private Person person;

        public Checker(ref Person p)
        {
            person = p;
        }

        public int getAbsoluteVelocity(JointType type)
        {
            return 0;
        }

        public int getRelativeVelocity(JointType t1, JointType t2)
        {
            return 0;
        }

        public int getDistance(JointType t1, JointType t2)
        {
            return 0;
        }

        public List<Direction> getAbsoluteMovement(JointType type)
        {
            return null;
        }

        public List<Direction> getRelativeMovement(JointType steady, JointType moving)
        {
            return null;
        }
    }
}
