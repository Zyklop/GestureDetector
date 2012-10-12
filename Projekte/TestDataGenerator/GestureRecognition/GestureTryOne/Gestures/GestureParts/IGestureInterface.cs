// -----------------------------------------------------------------------
// <copyright file="IGestureInterface.cs" company="Microsoft Limited">
//  Copyright (c) Microsoft Limited, Microsoft Consulting Services, UK. All rights reserved.
// All rights reserved.
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>Description</summary>
//-----------------------------------------------------------------------
namespace KinectSkeletonTracker.Gestures
{
    #region using...

    using Microsoft.Kinect;

    #endregion

    /// <summary>
    /// Defines a single gesture segment which uses relative positioning 
    /// of body parts to detect a gesture
    /// </summary>
    public interface IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        GesturePartResult CheckGesture(Skeleton skeleton);
    }
}
