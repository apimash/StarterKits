using APIMASH_RottenTomatoesLib;
using APIMASHLib;
using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_RottenTomatoes_StarterKit
{
    public sealed partial class GroupedItemsPage : APIMASH_RottenTomatoes_StarterKit.Common.LayoutAwarePage
    {
        private readonly APIMASHInvoke _apiInvokeInTheaters;

        private static bool _loaded = false;

        public GroupedItemsPage()
        {
            this.InitializeComponent();
            _apiInvokeInTheaters = new APIMASHInvoke();
            _apiInvokeInTheaters.OnResponse += apiInvoke_OnResponseInTheaters;

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
            var movieGroups = APIMASH_RottenTomatoesCollection.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = movieGroups;

            if (!_loaded) Invoke();
        }

        void Header_Click(object sender, RoutedEventArgs e)
        {
            //// Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;

            //// Navigate to the appropriate destination page, configuring the new page
            //// by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(GroupDetailPage), ((MovieGroup)group).UniqueId);
        }

        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((MovieItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }

        private void Invoke()
        {

            // STEP 2. Change the API here to see different movie listings
            var apiCall = Globals.ROTTEN_TOMATOES_API_MOVIES_INTHEATERS;
            //var apiCall = Globals.ROTTEN_TOMATOES_API_DVD_TOPRENTALS;
            _apiInvokeInTheaters.Invoke<RottenTomatoesMovies>(apiCall);
        }

        async private void apiInvoke_OnResponseInTheaters(object sender, APIMASHEvent e)
        {
            var response = (RottenTomatoesMovies)e.Object;

            if (e.Status == APIMASHStatus.SUCCESS)
            {
                // copy data into bindable format for UI
                APIMASH_RottenTomatoesCollection.Copy(response, System.Guid.NewGuid().ToString(), "In Theaters");
                //APIMASH_RottenTomatoesCollection.Copy(response, System.Guid.NewGuid().ToString(), "DVD Top Rentals");
                this.DefaultViewModel["AllGroups"] = APIMASH_RottenTomatoesCollection.GetGroups("AllGroups");
                _loaded = true;
            }
            else
            {
                var md = new MessageDialog(e.Message, "Error");
                bool? result = null;
                md.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler((cmd) => result = true)));
                await md.ShowAsync(); 
            }
        }
    }
}
