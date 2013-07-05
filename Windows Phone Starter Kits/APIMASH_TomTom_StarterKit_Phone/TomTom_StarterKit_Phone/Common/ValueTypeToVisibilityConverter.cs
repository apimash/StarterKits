using System;
using System.Windows;
using System.Windows.Data;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace TomTom_StarterKit_Phone.Common
{
    public sealed class ValueTypeToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Boolean)
                return ((Boolean)value) ? Visibility.Visible : Visibility.Collapsed;

            if (value is Int32)
                return ((Int32)value == 0) ? Visibility.Collapsed : Visibility.Visible;

            if (value is Double)
                return ((Double)value == 0) ? Visibility.Collapsed : Visibility.Visible;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
