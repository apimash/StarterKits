using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using APIMASH_RottenTomatoesLib;
using APIMASHLib;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Windows.System;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_RottenTomatoes_StarterKit
{
    public sealed partial class ItemDetailPage : APIMASH_RottenTomatoes_StarterKit.Common.LayoutAwarePage
    {
        private readonly APIMASHInvoke apiInvokePreview; 
        private readonly APIMASHInvoke apiInvokeReviews;

        public ItemDetailPage()
        {
            this.InitializeComponent();

            apiInvokePreview = new APIMASHInvoke();
            apiInvokeReviews = new APIMASHInvoke();

            apiInvokePreview.OnResponse += apiInvokePreviews_OnResponse;
            apiInvokeReviews.OnResponse += apiInvokeReviews_OnResponse;

            var settingsPane = SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested += settingsPane_CommandsRequested;
        }

        void settingsPane_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            // update to supply links to your about, support and privavy policy web pages
            var aboutCmd = new SettingsCommand("About", "About", (x) => Launcher.LaunchUriAsync(new Uri("")));
            var supportCmd = new SettingsCommand("Support", "Support", (x) => Launcher.LaunchUriAsync(new Uri("")));
            var policyCmd = new SettingsCommand("PrivacyPolicy", "Privacy Policy", (x) => Launcher.LaunchUriAsync(new Uri("")));

            args.Request.ApplicationCommands.Add(aboutCmd);
            args.Request.ApplicationCommands.Add(supportCmd);
            args.Request.ApplicationCommands.Add(policyCmd);
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            var mi = APIMASH_RottenTomatoesCollection.GetItem((string)navigationParameter);
            this.DefaultViewModel["Item"] = mi;
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var mi = (MovieItem)this.DefaultViewModel["Item"];
            pageState["SelectedItem"] = mi.UniqueId;
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            var mi = (MovieItem)this.DefaultViewModel["Item"];

            var apiCall = mi.Clips + "?" + Globals.ROTTEN_TOMATOES_API_KEY;
            apiInvokePreview.Invoke<MoviePreviews>(apiCall);
        }

        async void apiInvokePreviews_OnResponse(object sender, APIMASHEvent e)
        {
            var cc = (PreviewControl)PreviewPopup.Child;
            var response = (MoviePreviews)e.Object;

            if ((e.Status == APIMASHStatus.SUCCESS) && (response.Clips != null))
            {
                PreviewPopup.IsOpen = true;
                cc.Navigate(response.Clips[0].Links.Alternate);
            }
            else
            {
                if (response.Clips == null)
                    e.Message = "There are no previews";
                var md = new MessageDialog(e.Message, "Error");
                bool? result = null;
                md.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler((cmd) => result = true)));
                await md.ShowAsync(); 
            }
        }

        private void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            var mi = (MovieItem)this.DefaultViewModel["Item"];
            var apiCall = mi.Reviews + "?" + Globals.ROTTEN_TOMATOES_API_KEY;
            apiInvokeReviews.Invoke<MovieReviews>(apiCall);
        }

        async void apiInvokeReviews_OnResponse(object sender, APIMASHEvent e)
        {
            var cc = (ReviewControl)ReviewPopup.Child;
            var response = (MovieReviews)e.Object;

            if ((e.Status == APIMASHStatus.SUCCESS) && (response.Reviews.Length > 0))
            {
                var mg = new MovieReviewGroup();
                mg.Copy(response);
                cc.MovieReviews = mg;
                cc.Initialize();
                ReviewPopup.IsOpen = true;
            }
            else
            {
                if (response.Reviews.Length <= 0)
                    e.Message = "There are no previews";
                var md = new MessageDialog(e.Message, "Error");
                bool? result = null;
                md.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler((cmd) => result = true)));
                await md.ShowAsync(); // issue here intermitment
            }
        }
    }
}
