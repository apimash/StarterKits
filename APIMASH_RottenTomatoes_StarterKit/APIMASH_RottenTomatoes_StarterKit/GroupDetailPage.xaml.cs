using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using APIMASH_RottenTomatoesLib;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Windows.System;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_RottenTomatoes_StarterKit
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    public sealed partial class GroupDetailPage : APIMASH_RottenTomatoes_StarterKit.Common.LayoutAwarePage
    {
        public GroupDetailPage()
        {
            this.InitializeComponent();
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

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var group = APIMASH_RottenTomatoesCollection.GetGroupById((String)navigationParameter);
            this.DefaultViewModel["Group"] = group;
            this.DefaultViewModel["Items"] = group.Items;
        }

        /// <summary>
        /// Invoked when an item is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((MovieItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }
    }
}
