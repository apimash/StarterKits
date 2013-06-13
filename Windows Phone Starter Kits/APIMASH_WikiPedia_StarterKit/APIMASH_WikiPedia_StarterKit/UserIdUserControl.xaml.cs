// LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt   <== yes, there's a space in it, dont ask....
// APIMash - http://bit.ly/apimash
// Joe Healy / jhealy@microsoft.com / josephehealy@hotmail.com / @devfish

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using APIMASH_WikiPedia_StarterKit.Common;

// Requires CharmsStyles.xaml to be in the common director and also added to the resource dictionareis 
//      section in App.xaml
namespace APIMASH_WikiPedia_StarterKit
{
    public sealed partial class UserIdUserControl : UserControl
    {
        public UserIdUserControl()
        {
            this.InitializeComponent();
            this.Loaded += UserIdUserControl_Loaded;
            this.Unloaded += UserIdUserControl_Unloaded;
        }

        void UserIdUserControl_Unloaded(object sender, RoutedEventArgs e)
        {
                // nothing.  saved via save button
        }

        void UserIdUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string _userid = APIMASHGlobals.Instance.UserID;
            this.TextBlock_UserID.Text = _userid;
            TextBlock_NoUserId.Visibility = APIMASHGlobals.Instance.UserID.Length > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Button_GoBack_Click(object sender, RoutedEventArgs e)
        {
            CloseThis();           
        }

        private void CloseThis()
        {
            // First close our Flyout.
            Popup parent = this.Parent as Popup;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            // If the app is not snapped, then the back button shows the Settings pane again.
            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
        }

        private async void HyperLinkButton_CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            Uri _uri = new Uri(@"http://www.geonames.org/login");
            await Launcher.LaunchUriAsync(_uri);
        }

        private void Button_StoreUserId_Click(object sender, RoutedEventArgs e)
        {
            string _userid = TextBlock_UserID.Text;
            if (_userid.Length <= 0)
            {
                devfish.utils.MessageDialogHelper.ShowMsg("user name required", "the user name cannot be blank");
            }

            APIMASHGlobals.Instance.UserID = _userid;
            App.Current.Resources["GeonamesUserName"] = _userid; // this is what apps bind to

            System.Diagnostics.Debug.WriteLine("userid stored : " + _userid);
            CloseThis();
        }
    }
}

