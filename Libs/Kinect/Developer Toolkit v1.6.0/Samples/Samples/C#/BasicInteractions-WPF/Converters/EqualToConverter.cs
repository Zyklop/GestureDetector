//------------------------------------------------------------------------------
// <copyright file="EqualToConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BasicInteractions
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class EqualToConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            bool result = true;
            switch (values.Length)
            {
                case 0:
                    result = false;
                    break;
                case 1:
                    result = true;
                    break;
                default:
                    {
                        for (int i = 1; i < values.Length; i++)
                        {
                            if (values[0].Equals(values[i]) == false)
                            {
                                result = false;
                                break;
                            }
                        }
                    }

                    break;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}