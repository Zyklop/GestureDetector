//------------------------------------------------------------------------------
// <copyright file="MillisecondsToDurationConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class MillisecondsToDurationConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double milliseconds = 0;
            if (value is double)
            {
                milliseconds += (double)value;
                if (parameter is double)
                {
                    milliseconds += (double)parameter;
                }
            }

            return new Duration(TimeSpan.FromMilliseconds(milliseconds));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}