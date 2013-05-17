using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

//
// LICENSE: http://opensource.org/licenses/ms-pl) 
//

namespace APIMASH_StarterKit.Common
{
    /// <summary>
    /// Value converter that translates an empty or null string value to <see cref="Visibility.Collapsed"/>
    /// </summary>
    public sealed class EmptyStringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return String.IsNullOrEmpty(value as String) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
