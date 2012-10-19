using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace DataSources
{
    class SmothendSkeleton
    {
        private Dictionary<JointType, Joint> joints;

        public SmothendSkeleton(Skeleton s)
        {
            joints = new Dictionary<JointType, Joint>();
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
