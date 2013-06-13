using APIMASH_CNorrisLib;
using APIMASHLib;
using System;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Facebook;
using System.Collections.Generic;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_Facebook_StarterKit
{
    public sealed partial class MainPage : LayoutAwarePage
    {
        readonly APIMASHInvoke apiInvoke;
        string name = "&firstName=Chuck&lastName=Norris"; //to append to API call based on Friend
        string id = "random"; //to choose joke randomly or by its ID

        public MainPage()
        {
            this.InitializeComponent();
            apiInvoke = new APIMASHInvoke();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loginButton.ApplicationId = Globals.FACEBOOK_API_KEY;
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
            apiInvoke.OnResponse += apiInvoke_OnResponse;
            string apiCall = @"http://api.icndb.com/jokes/"+id+"?exclude=[explicit]"+ name;
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
                id = response.Value.Id;
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
            id = "random";
            name = "&firstName=" + FriendPicker.SelectedItem.FirstName + "&lastName=" + FriendPicker.SelectedItem.LastName;
            Joke.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Invoke();
        }

        private void loginButton_SessionStateChanged(object sender, Facebook.Client.Controls.SessionStateChangedEventArgs e)
        {
            if (e.SessionState == Facebook.Client.Controls.FacebookSessionState.Opened)
            {
                Welcome.Text = "Welcome, ";
                HitMe.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Post.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                Welcome.Text = "Please Login";
                HitMe.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Post.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        async private void Post_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await this.loginButton.RequestNewPermissions("publish_actions");

            var facebookClient = new Facebook.FacebookClient(this.loginButton.CurrentSession.AccessToken);

            var postParams = new {
                message = Joke.Text
            };

            try
            {
                // me/feed posts to logged in user's feed
                dynamic fbPostTaskResult = await facebookClient.PostTaskAsync("/me/feed", postParams);
                var result = (IDictionary<string, object>)fbPostTaskResult;

                var successMessageDialog = new Windows.UI.Popups.MessageDialog("Posted Open Graph Action, id: " + (string)result["id"]);
                await successMessageDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var exceptionMessageDialog = new Windows.UI.Popups.MessageDialog("Exception during post: " + ex.Message);
                exceptionMessageDialog.ShowAsync();
            }
         }

        //gets a new Joke, with a custom name based on your selected Facebook friend
        private void FriendPicker_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            name = "&firstName=" + FriendPicker.SelectedItem.FirstName + "&lastName=" + FriendPicker.SelectedItem.LastName;
            Joke.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Invoke();
        }

      }
}