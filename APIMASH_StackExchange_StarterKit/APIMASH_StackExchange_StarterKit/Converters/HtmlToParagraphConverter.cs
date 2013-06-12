using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;

namespace APIMASH_StackExchange_StarterKit.Converters
{
    public class HtmlToParagraphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                Paragraph para = new Paragraph();
                var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(value.ToString());

                using (MemoryStream stream = new MemoryStream((new UTF8Encoding()).GetBytes(xaml)))
                {
                    //para. text = new TextRange(para.ContentStart, para.ContentEnd);
                    //text.Load(stream, DataFormats.Xaml);
                }

                return para;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
