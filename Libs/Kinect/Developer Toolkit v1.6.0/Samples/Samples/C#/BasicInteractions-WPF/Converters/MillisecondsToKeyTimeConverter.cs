//------------------------------------------------------------------------------
// <copyright file="MillisecondsToKeyTimeConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Animation;

    public class MillisecondsToKeyTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            KeyTime keyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
            if (value is double)
            {
                keyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((double)value));
                if (parameter is double)
                {
                    keyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((double)value + (double)parameter));
                }
            }

            return keyTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}