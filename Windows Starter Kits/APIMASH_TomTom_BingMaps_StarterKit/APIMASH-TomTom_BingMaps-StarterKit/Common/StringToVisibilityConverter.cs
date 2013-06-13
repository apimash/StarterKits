using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH_StarterKit.Common
{
    public sealed class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            String s = value as String;
            if (String.IsNullOrEmpty(s))
                return Visibility.Collapsed;
            else
                return (s.Trim().Length > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
