using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace APIMASH_WikiPedia_StarterKit
{
    public sealed partial class FooterUserControl : UserControl
    {
        public FooterUserControl()
        {
            this.InitializeComponent();
        }
        public void SetStatus( string msg )
        {
            TextBlock_Status.Text = msg;
        }
    }
}
