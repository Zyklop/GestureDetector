using System.Collections.Generic;
using Microsoft.Kinect;

namespace MF.Engineering.MF8910.GestureDetector.DataSources
{
    /// <summary>
    /// The joint data is stored here.
    /// Filtering is also possible here
    /// </summary>
    public class SmothendSkeleton
    {
        // The joint's data
        private Dictionary<JointType, Joint> joints;
        /// <summary>
        /// Time of the Skeleton
        /// </summary>
        public long Timestamp { get; private set; }

        public SmothendSkeleton(Skeleton s, long timestamp)
        {
            joints = new Dictionary<JointType, Joint>();
            Timestamp = timestamp;
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
