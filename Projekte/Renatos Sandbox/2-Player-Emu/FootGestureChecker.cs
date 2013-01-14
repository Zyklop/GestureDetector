using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emulator;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures;
using MF.Engineering.MF8910.GestureDetector.Tools;
using Microsoft.Kinect;

namespace _2_Player_Emu
{
    class FootGestureChecker : GestureChecker
    {
        protected const int ConditionTimeout = 2500;

        public FootGestureChecker(Person p)
            : base(new List<Condition> {

                new FootCondition(p)

            }, ConditionTimeout)
        {
        }
    }

    internal class FootCondition : Condition
    {
        private Person _person;

        public FootCondition(Person person) : base(person)
        {
            _person = person;
        }

        protected override void Check(object src, NewSkeletonEventArgs e)
        {
            double dist = _person.CurrentSkeleton.GetPosition(JointType.FootLeft).Z -
                          _person.CurrentSkeleton.GetPosition(JointType.FootRight).Z;
            if (dist > 0.2)
            {
                FireSucceeded(this, new FootEventArgs{Distance = dist, Foot = Direction.Left});
            }
            if (dist < -0.2)
            {
                FireSucceeded(this, new FootEventArgs { Distance = -dist, Foot = Direction.Right });
            }
        }
    }
}
