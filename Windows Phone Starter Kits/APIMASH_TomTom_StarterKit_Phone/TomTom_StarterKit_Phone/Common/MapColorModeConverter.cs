using Microsoft.Phone.Maps.Controls;
using System;
using System.Windows.Data;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace TomTom_StarterKit_Phone.Common
{
    public sealed class MapColorModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Boolean)
                return ((Boolean)value ? MapColorMode.Light : MapColorMode.Dark);
            return MapColorMode.Light;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is MapColorMode)
                return ((MapColorMode) value == MapColorMode.Light);
            return false;
        }
    }
}
