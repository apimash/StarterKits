using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH_StarterKit.Common
{
    public sealed class ValueTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Boolean)
                return ((Boolean)value) ? Visibility.Visible : Visibility.Collapsed;

            if (value is Int32)
                return ((Int32)value == 0) ? Visibility.Collapsed : Visibility.Visible;

            if (value is Double)
                return ((Int32)value == 0) ? Visibility.Collapsed : Visibility.Visible;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
