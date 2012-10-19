//------------------------------------------------------------------------------
// <copyright file="ContentItem.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Windows.Media;

    public class ContentItem
    {
        public ContentItem()
        {
            this.Rating = new Rating();
        }

        public string Title { get; set; }

        public Category Category { get; set; }

        public string Subcategory { get; set; }

        public string Content { get; set; }

        public ImageSource ContentImage { get; set; }

        public Uri ContentVideo { get; set; }

        public Rating Rating { get; set; }

        public int ItemId { get; set; }
    }
}