//------------------------------------------------------------------------------
// <copyright file="HandInputEventArgs.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System.Windows;

    public class HandInputEventArgs : RoutedEventArgs
    {
        public HandInputEventArgs()
        {
        }

        public HandInputEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
        }

        public HandInputEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }

        public HandInputEventArgs(RoutedEvent routedEvent, object source, HandPosition hand)
            : base(routedEvent, source)
        {
            this.Hand = hand;
        }

        public HandPosition Hand { get; set; }
    }
}