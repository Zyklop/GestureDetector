using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Conditions;
using DataSources;

namespace Gesture_Detector
{
    class GestureChecker
    {
        List<ICondition> conditions;

        public GestureChecker (List<ICondition> cl)
        {
            conditions = cl;
        }
    }
}
