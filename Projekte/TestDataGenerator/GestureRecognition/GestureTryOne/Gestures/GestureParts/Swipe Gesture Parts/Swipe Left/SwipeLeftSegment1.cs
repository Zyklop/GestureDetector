// -----------------------------------------------------------------------
// <copyright file="SwipeLeftSegment1.cs" company="Microsoft Limited">
//  Copyright (c) Microsoft Limited, Microsoft Consulting Services, UK. All rights reserved.
// All rights reserved.
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The first part of the swipe left gesture</summary>
//-----------------------------------------------------------------------
namespace KinectSkeletonTracker.Gestures.GestureParts
{
    #region using...

    using Microsoft.Kinect;

    #endregion

    /// <summary>
    /// The first part of the swipe left gesture
    /// </summary>
    public class SwipeLeftSegment1 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // //Right hand in front of right shoulder
            if (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ElbowRight].Position.Z && skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.HipCenter].Position.Y)
            {
                // Console.WriteLine("GesturePart 0 - Right hand in front of right shoudler - PASS");
                // //right hand below shoulder height but above hip height
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.Head].Position.Y && skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
                {
                    // Console.WriteLine("GesturePart 0 - right hand below shoulder height but above hip height - PASS");
                    // //right hand right of right shoulder
                    if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderRight].Position.X)
                    {
                        // Console.WriteLine("GesturePart 0 - right hand right of right shoulder - PASS");
                        return GesturePartResult.Succeed;
                    }

                    // Console.WriteLine("GesturePart 0 - right hand right of right shoulder - UNDETERMINED");
                    return GesturePartResult.Pausing;
                }

                // Console.WriteLine("GesturePart 0 - right hand below shoulder height but above hip height - FAIL");
                return GesturePartResult.Fail;
            }

            // Console.WriteLine("GesturePart 0 - Right hand in front of right shoulder - FAIL");
            return GesturePartResult.Fail;
        }
    }
}