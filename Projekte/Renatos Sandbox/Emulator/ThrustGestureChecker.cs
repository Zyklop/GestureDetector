using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Gestures;
using MF.Engineering.MF8910.GestureDetector.Tools;
using Microsoft.Kinect;

namespace Emulator
{
    public class ThrustGestureChecker:GestureChecker
    {
        protected const int ConditionTimeout = 2500;

        public ThrustGestureChecker(Person p)
            : base(new List<Condition> {

                new ThrustingCondition(p)

            }, ConditionTimeout)
        {
        }
    }

    internal class ThrustingCondition : Condition
    {
        private Person person;

        public ThrustingCondition(Person person):base(person)
        {
            this.person = person;
        }

        protected override void Check(object src, NewSkeletonEventArgs e)
        {
            double dist = person.CurrentSkeleton.GetPosition(JointType.ShoulderCenter).Z - person.CurrentSkeleton.GetPosition(JointType.HandRight).Z;
                FireSucceeded(this, new ThrustGestureEventArgs{DistanceToShoulder = dist});
        }
    }
}
