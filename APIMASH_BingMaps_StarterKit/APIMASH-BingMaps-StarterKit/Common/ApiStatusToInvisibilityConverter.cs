using APIMASH;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

/*
 * LICENSE: http://opensource.org/licenses/ms-pl) 
 */

namespace APIMASH_BingMaps_StarterKit.Common
{
    /// <summary>
    /// Value converter that translates successful statuses of HTTP calls to <see cref="Visibility.Collapsed"/> and to
    /// <see cref="Visibility.Visible"/> otherwise.
    /// </summary>
    public sealed class ApiStatusToInvisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ApiResponseStatus s = value as ApiResponseStatus;
            if (s == null || s.IsSuccessStatusCode)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
