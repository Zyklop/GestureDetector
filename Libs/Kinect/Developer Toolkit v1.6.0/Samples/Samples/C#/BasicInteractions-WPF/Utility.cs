//------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System.Windows;
    using System.Windows.Media;

    public static class Utility
    {
        public static T FindParent<T>(object child)
            where T : DependencyObject
        {
            var search = child as DependencyObject;
            T parent = null;
            while (search != null && (parent = search as T) == null)
            {
                search = VisualTreeHelper.GetParent(search);
            }

            return parent;
        }

        public static bool IsElementChild(DependencyObject parentElement, DependencyObject childElement)
        {
            DependencyObject search = childElement;
            while (search != null && search != parentElement)
            {
                search = VisualTreeHelper.GetParent(search);
            }

            return search != null;
        }
    }
}