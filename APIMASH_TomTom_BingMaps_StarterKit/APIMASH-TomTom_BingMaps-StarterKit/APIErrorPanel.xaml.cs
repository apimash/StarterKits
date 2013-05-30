using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH_StarterKit
{
    /// <summary>
    /// Control that presents HTTP Status code and message to display message after a failed API call
    /// </summary>
    public sealed partial class ApiErrorPanel : UserControl
    {
        /// <summary>
        /// Occurs when user taps the dismissed button to hide the panel
        /// </summary>
        public event EventHandler Dismissed;
        private void OnDismissed(EventArgs e)
        {
            if (Dismissed != null) Dismissed(this, e);
        }

        private void Dismiss_Tapped(object sender, TappedRoutedEventArgs e)
        {
            OnDismissed(new EventArgs());
        }

        public ApiErrorPanel()
        {
            this.InitializeComponent();
        }
    }
}
