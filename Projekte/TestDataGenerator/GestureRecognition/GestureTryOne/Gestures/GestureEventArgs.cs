// -----------------------------------------------------------------------
// <copyright file="GestureEventArgs.cs" company="Microsoft Limited">
//  Copyright (c) Microsoft Limited, Microsoft Consulting Services, UK. All rights reserved.
// All rights reserved.
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The gesture event arguments</summary>
//-----------------------------------------------------------------------
namespace KinectSkeletonTracker
{
    #region using...

    using System;
    using KinectSkeletonTracker.Gestures;

    #endregion

    /// <summary>
    /// The gesture event arguments
    /// </summary>
    public class GestureEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GestureEventArgs"/> class.
        /// </summary>
        /// <param name="type">The gesture type.</param>
        /// <param name="trackingID">The tracking ID.</param>
        public GestureEventArgs(GestureType type, int trackingID)
        {
            this.TrackingID = trackingID;
            this.GestureType = type;
        }

        /// <summary>
        /// Gets or sets the type of the gesture.
        /// </summary>
        /// <value>
        /// The type of the gesture.
        /// </value>
        public GestureType GestureType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tracking ID.
        /// </summary>
        /// <value>
        /// The tracking ID.
        /// </value>
        public int TrackingID
        {
            get;
            set;
        }
    }
}
