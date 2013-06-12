using APIMASH_CNorrisLib;
using APIMASHLib;
using System;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_CNorris_StarterKit
{
    public sealed partial class MainPage : LayoutAwarePage
    {
        readonly APIMASHInvoke apiInvoke;

        public MainPage()
        {
            this.InitializeComponent();
            apiInvoke = new APIMASHInvoke();
            apiInvoke.OnResponse += apiInvoke_OnResponse;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var settingsPane = Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested += settingsPane_CommandsRequested;
            Invoke();
        }

        ///////////////////////////////////////////////////////////////////////////////////
        // Update with URLs to About, Support and Privacy Policy Web Pages
        ///////////////////////////////////////////////////////////////////////////////////
        void settingsPane_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var aboutCmd = new SettingsCommand("About", "About", (x) => Launcher.LaunchUriAsync(new Uri("")));
            var supportCmd = new SettingsCommand("Support", "Support", (x) => Launcher.LaunchUriAsync(new Uri("")));
            var policyCmd = new SettingsCommand("PrivacyPolicy", "Privacy Policy", (x) => Launcher.LaunchUriAsync(new Uri("")));

            args.Request.ApplicationCommands.Add(aboutCmd);
            args.Request.ApplicationCommands.Add(supportCmd); 
            args.Request.ApplicationCommands.Add(policyCmd);
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // Update this routine to build the URI to invoke the API 
        // determine how you want to build the API call: 
        //     a) using user input
        //     b) hard coded values
        //     c) all of the above
        ///////////////////////////////////////////////////////////////////////////////////
        private void Invoke()
        {
            const string apiCall = @"http://api.icndb.com/jokes/random?exclude=[explicit]";
            apiInvoke.Invoke<CNorrisJoke>(apiCall);
        }

        async private void apiInvoke_OnResponse(object sender, APIMASHEvent e)
        {
            var response = (CNorrisJoke)e.Object;

            if (e.Status == APIMASHStatus.SUCCESS)
            {
                var s = response.Value.Joke;
                s = s.Replace("&quot;", "'");
                Joke.Text = s;
            }
            else
            {
                var md = new MessageDialog(e.Message, "Error");
                bool? result = null;
                md.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler((cmd) => result = true)));
                await md.ShowAsync();
            }
        }

        private void HitMeButtonClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Invoke();
        }
    }
}