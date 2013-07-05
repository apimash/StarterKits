using System;
using System.Windows;
using System.Windows.Data;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace TomTom_StarterKit_Phone.Common
{
    public sealed class ObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
