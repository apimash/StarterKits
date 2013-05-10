using System;
using Windows.UI.Xaml.Data;
using Windows.UI;
using Windows.UI.Xaml.Media;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH.Diagnostics
{
    class ApiStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ApiResponseStatus ars = value as ApiResponseStatus;
            if (ars != null)
            {
                if (ars.CausedException)
                    return new SolidColorBrush(Colors.Red);
                if (ars.IsSuccessStatusCode)
                    return new SolidColorBrush(Colors.Green);
                else
                    return new SolidColorBrush(Colors.DarkOrange);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
