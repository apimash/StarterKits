using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_RottenTomatoes_StarterKit
{
    public sealed partial class PreviewControl : UserControl
    {
        public PreviewControl()
        {
            this.InitializeComponent();

            var bounds = Window.Current.Bounds;
            this.RootPanel.Width = bounds.Width;
            this.RootPanel.Height = bounds.Height - 200;
        }

        public void Navigate(string url)
        {
            try
            {
                var page = new Uri(url);
                PreviewPage.Navigate(page);
            }
            catch (Exception)
            {
                ;
            }

        }

        private void ClipsControl_OnTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var parent = (Popup)this.Parent;
            parent.IsOpen = false;
        }
    }
}
