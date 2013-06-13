/*
 * LICENSE: http://opensource.org/licenses/ms-pl 
 */

using APIMASH_YelpLib;
using APIMASHLib;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace APIMASH_Yelp_StarterKit
{
    public sealed partial class MainPage : LayoutAwarePage
    {
        APIMASHInvoke apiInvoke;

        public MainPage()
        {
            this.InitializeComponent();

            apiInvoke = new APIMASHInvoke();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane settingsPane = Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested += settingsPane_CommandsRequested;

            Invoke(Term.Text, Location.Text);
        }

        ///////////////////////////////////////////////////////////////////////////////////
        // Update with URLs to About, Support and Privacy Policy Web Pages
        ///////////////////////////////////////////////////////////////////////////////////
        void settingsPane_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            // update to supply links to your about, support and privavy policy web pages
            var aboutCmd = new SettingsCommand("About", "About", (x) => Launcher.LaunchUriAsync(new Uri("http://YOUR_SITE_HERE/About.html")));
            var supportCmd = new SettingsCommand("Support", "Support", (x) => Launcher.LaunchUriAsync(new Uri("http://YOUR_SITE_HERE/Contact.html")));
            var policyCmd = new SettingsCommand("PrivacyPolicy", "Privacy Policy", (x) => Launcher.LaunchUriAsync(new Uri("http://YOUR_SITE_HERE/PrivacyPolicy.html")));

            args.Request.ApplicationCommands.Add(aboutCmd);
            args.Request.ApplicationCommands.Add(supportCmd);
            args.Request.ApplicationCommands.Add(policyCmd);
        }

        private void Invoke(string term, string location)
        {
            Progress.IsActive = true;

            apiInvoke.OnResponse += apiInvoke_OnResponse;

            // NOTE: This sample uses v1.0 of Yelp's API.  There is 
            //   also a v2.0 API, supporting OAUTH and additional functionality

            // Yelp 1.0 DEV KEY
            string YELP_API_KEY = @"ywsid=[YOUR-DEV-KEY-HERE]";

            // By default, call Business Review Search and limit to open businesses
            // The API offers many other services/options
            string apiCall = @"http://api.yelp.com/business_review_search?is_closed=false&term=" + term + "&location=" + location + "&" + YELP_API_KEY;

            apiInvoke.Invoke<Yelp_Response>(apiCall);
        }

        async private void apiInvoke_OnResponse(object sender, APIMASHEvent e)
        {
            Progress.IsActive = false;
            Yelp_Response response = (Yelp_Response)e.Object;

            if ((e.Status == APIMASHStatus.SUCCESS) && (response.businesses.Length > 0))
            {
                var bg = new BusinessGroup();
                bg.Copy(response.businesses);
                BusinessGridView.ItemsSource = bg.Items;
            }
            else
            {
                MessageDialog md = new MessageDialog(e.Message, "Error");
                bool? result = null;
                md.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler((cmd) => result = true)));
                await md.ShowAsync();
            }
        }

        private void SearchButtonClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Invoke(Term.Text, Location.Text);
        }

    }
}