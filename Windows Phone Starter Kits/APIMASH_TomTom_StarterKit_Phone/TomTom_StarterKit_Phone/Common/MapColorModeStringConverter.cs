using System;
using System.Windows.Data;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace TomTom_StarterKit_Phone.Common
{
    public sealed class MapColorModeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Boolean)
                return ((Boolean)value ? "Light" : "Dark");
            return "Light";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is String)
                return (value.ToString() == "Light");
            return false;
        }
    }
}
