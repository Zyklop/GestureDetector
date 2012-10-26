﻿using DataSources;
using GestureEvents;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Conditions.Login
{
    class WaveRightCondition: DynamicCondition
    {
        private Checker checker;

        public WaveRightCondition(Person p): base(p)
        {
            checker = new Checker(p);
        }

        protected override void check(object src, NewSkeletonEventArg e)
        {
            List<Direction> rightHandDirections = checker.GetAbsoluteMovement(JointType.HandRight);
            List<Direction> handToHeadDirections = checker.GetRelativePosition(JointType.Head, JointType.HandRight);

            rightHandDirections.ForEach(delegate(Direction x) { Debug.Write(x.ToString()); });

            // Prüfe ob Handbewegung nach links abläuft und ob sich die Hand über dem Kopf befindet
            if (rightHandDirections.Contains(Direction.right) && handToHeadDirections.Contains(Direction.upward))
            {
                fireTriggered(this, null);
                Debug.WriteLine("WaveLeft recognized.");
                //fireRecognized(this, null);
            }
        }
    }
}
