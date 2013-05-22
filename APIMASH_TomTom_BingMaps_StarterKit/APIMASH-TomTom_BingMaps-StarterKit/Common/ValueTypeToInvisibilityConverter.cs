using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

//
// LICENSE: http://opensource.org/licenses/ms-pl) 
//

namespace APIMASH_StarterKit.Common
{
    public sealed class ValueTypeToInvisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Boolean)
                return ((Boolean)value) ? Visibility.Collapsed : Visibility.Visible;

            if (value is Int32)
                return ((Int32)value == 0) ? Visibility.Visible : Visibility.Collapsed;

            if (value is Double)
                return ((Int32)value == 0) ? Visibility.Visible : Visibility.Collapsed;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
