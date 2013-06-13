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
    public class HtmlToRichTextBlockConverter : IValueConverter
    {
        private const string pre = "<RichTextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" x:Name=\"richTextBlock\" Width=\"640\" Style=\"{StaticResource ItemRichTextStyle}\" IsTextSelectionEnabled=\"False\"><Paragraph><Run FontSize=\"26.667\" FontWeight=\"Light\" Text=\"{Binding Title}\"/><LineBreak/><LineBreak/></Paragraph>";
        private const string post = "</RichTextBlock>";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var xaml = pre + HtmlToXamlConverter.ConvertHtmlToXaml(value.ToString()) + post;
                var root = Windows.UI.Xaml.Markup.XamlReader.Load(xaml);
                return root;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
