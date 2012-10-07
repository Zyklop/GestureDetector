// -----------------------------------------------------------------------
// <copyright file="GestureEnumTypes.cs" company="Microsoft Limited">
//  Copyright (c) Microsoft Limited, Microsoft Consulting Services, UK. All rights reserved.
// All rights reserved.
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>Different enums for the gesture service</summary>
//-----------------------------------------------------------------------
namespace KinectSkeletonTracker.Gestures
{
    /// <summary>
    /// the gesture part result
    /// </summary>
    public enum GesturePartResult 
    {
        /// <summary>
        /// Gesture part fail
        /// </summary>
        Fail,

        /// <summary>
        /// Gesture part suceed
        /// </summary>
        Succeed,

        /// <summary>
        /// Gesture part result undetermined
        /// </summary>
        Pausing 
    }

    /// <summary>
    /// The gesture type
    /// </summary>
    public enum GestureType 
    {
        /// <summary>
        /// Waved with right hand
        /// </summary>
        WaveRight,

        /// <summary>
        /// Waved with the left hand
        /// </summary>
        WaveLeft,

        /// <summary>
        /// Asked for the menu
        /// </summary>
        Menu,

        /// <summary>
        /// Swiped left
        /// </summary>
        LeftSwipe,

        /// <summary>
        /// swiped right
        /// </summary>
        RightSwipe 
    }
}
