using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace MF.Engineering.MF8910.GestureDetector.DataSources
{
    public class SmothendSkeleton
    {
        private Dictionary<JointType, Joint> joints;
        private long t;
        public long Timestamp { get { return t; } }

        public SmothendSkeleton(Skeleton s, long timestamp)
        {
            joints = new Dictionary<JointType, Joint>();
            t = timestamp;
            foreach (Joint j in s.Joints)
            {
                joints.Add(j.JointType, j);
            }
        }

        public SkeletonPoint GetPosition(JointType jt)
        {
            return joints[jt].Position;
        }

        public JointTrackingState GetState(JointType jt)
        {
            return joints[jt].TrackingState;
        }
    }
}
