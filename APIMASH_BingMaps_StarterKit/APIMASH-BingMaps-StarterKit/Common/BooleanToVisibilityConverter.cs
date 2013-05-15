using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

/*
 * LICENSE: http://opensource.org/licenses/ms-pl) 
 */

namespace APIMASH_BingMaps_StarterKit.Common
{
    /// <summary>
    /// Value converter that determines visibility of image based on properties.
    /// </summary>
    public sealed class ImageToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BitmapImage image = value as BitmapImage;
            return (image != null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
