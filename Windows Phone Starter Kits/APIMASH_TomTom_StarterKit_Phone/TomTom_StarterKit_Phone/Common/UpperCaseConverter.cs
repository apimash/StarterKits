using System;
using System.Windows.Data;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace TomTom_StarterKit_Phone.Common
{
    public sealed class UpperCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is String)
                return ((String)value).ToUpper(culture);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}