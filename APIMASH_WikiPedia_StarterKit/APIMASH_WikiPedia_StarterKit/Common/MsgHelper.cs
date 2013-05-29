// LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt   <== yes, there's a space in it, dont ask....
// APIMash - http://bit.ly/apimash
// Joe Healy / jhealy@microsoft.com / josephehealy@hotmail.com / @devfish

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace APIMASH_WikiPedia_StarterKit.Common
{
    public class MsgHelper
    {
        private TextBlock m_textblock;

        public MsgHelper(TextBlock textblock)
        {
            m_textblock = textblock;
        }

        public void clr()
        {
            m_textblock.Text = string.Empty;
        }

        // default with a line feed
        public void msg(string s)
        {
            m_textblock.Text = string.Format("{0}\r\n{1}", m_textblock.Text, s);
        }
        public void abouttextblock()
        {
            this.msg( string.Format("textblock dimensions: h={0}, w={0}", m_textblock.Height, m_textblock.Width));
        }
    }
}
