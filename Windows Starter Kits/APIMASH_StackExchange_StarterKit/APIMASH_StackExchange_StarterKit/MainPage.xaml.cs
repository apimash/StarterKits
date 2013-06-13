using System.Collections.Generic;
using APIMASH_StackExchange_StarterKit.Common;
using APIMASHLib;
using APIMASH_StackExchangeLib;

using System;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace APIMASH_StackExchange_StarterKit
{
    public sealed partial class MainPage : LayoutAwarePage
    {
        APIMASHInvokeCompressed apiInvoke;
        private static bool _loaded = false;

        public MainPage()
        {
            this.InitializeComponent();

            apiInvoke = new APIMASHInvokeCompressed();
            apiInvoke.OnResponse += apiInvoke_OnResponse;
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            this.DefaultViewModel["Groups"] = APIMASH_StackExchangeCollection.GetGroups((String)navigationParameter); ;

            if (!_loaded) Invoke();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane settingsPane = Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested += settingsPane_CommandsRequested;
            base.OnNavigatedTo(e);
        }

        ///////////////////////////////////////////////////////////////////////////////////
        // Update with URLs to About, Support and Privacy Policy Web Pages
        ///////////////////////////////////////////////////////////////////////////////////
        void settingsPane_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var rl = new ResourceLoader();

            var aboutCmd = new SettingsCommand("About", rl.GetString("SETTINGS_PANEL_CMD_ABOUT"), async (x) =>
            {
                await Launcher.LaunchUriAsync(new Uri(""));
            });

            args.Request.ApplicationCommands.Add(aboutCmd);

            var supportCmd = new SettingsCommand("Support", rl.GetString("SETTINGS_PANEL_CMD_SUPPORT"), async (x) =>
            {
                await Launcher.LaunchUriAsync(new Uri(""));
            });

            args.Request.ApplicationCommands.Add(supportCmd);

            var policyCmd = new SettingsCommand("PrivacyPolicy", rl.GetString("SETTINGS_PANEL_CMD_PRIVACY_POLICY"), async (x) =>
            {
                await Launcher.LaunchUriAsync(new Uri(""));
            });

            args.Request.ApplicationCommands.Add(policyCmd);
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // Update this routine to build the URI to invoke the API 
        // determine how you want to fire this routine, user action or automatically
        ///////////////////////////////////////////////////////////////////////////////////
        private void Invoke()
        {
            string apiCall = @"http://api.stackexchange.com/2.1/questions?order=desc&sort=activity&site=stackoverflow&filter=withbody";
            apiInvoke.Invoke<StackExchangeQuestions>(apiCall);
        }

        async private void apiInvoke_OnResponse(object sender, APIMASHEvent e)
        {
            StackExchangeQuestions response = (StackExchangeQuestions)e.Object;

            if (e.Status == APIMASHStatus.SUCCESS)
            {
                // copy data into bindable format for UI
                APIMASH_StackExchangeCollection.Copy(response, System.Guid.NewGuid().ToString(), "All");
                this.DefaultViewModel["AllGroups"] = APIMASH_StackExchangeCollection.GetGroups("AllGroups");
                _loaded = true;
            }
            else
            {
                var md = new MessageDialog(e.Message, "Error");
                md.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler((cmd) => { })));
                await md.ShowAsync(); 
            }
        }

        void Header_Click(object sender, RoutedEventArgs e)
        {
            //// Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;

            //// Navigate to the appropriate destination page, configuring the new page
            //// by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(QuestionGroupDetailPage), ((QuestionGroup)group).Id);
        }

        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((QuestionItem)e.ClickedItem).Id;
            this.Frame.Navigate(typeof(QuestionItemDetailPage), itemId);
        }
    }
}
