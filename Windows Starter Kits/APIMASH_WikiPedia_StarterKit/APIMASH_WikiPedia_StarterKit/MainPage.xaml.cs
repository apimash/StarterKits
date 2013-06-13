// LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt   <== yes, there's a space in it, dont ask....
// APIMash - http://bit.ly/apimash
// Joe Healy / jhealy@microsoft.com / josephehealy@hotmail.com / @devfish
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using APIMASH_WikiPedia_StarterKit.Common;
using Windows.UI.Popups;
using Windows.UI.ApplicationSettings;
using Windows.System;
using Windows.UI;

namespace APIMASH_WikiPedia_StarterKit
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : APIMASH_WikiPedia_StarterKit.Common.LayoutAwarePage
    {
        // CHARMS
        Popup m_useridPopup;
        double m_CharmsFlyoutWidth = 346;
        Rect m_windowBounds;
        private Color m_background = Color.FromArgb(255, 0, 77, 96);

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;

            // CHARMS
            m_windowBounds = Window.Current.Bounds;
            Window.Current.SizeChanged += OnWindowSizeChanged;
            SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;
        }

        private void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            m_windowBounds = Window.Current.Bounds;
        }

        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Clear();

            var _privacy = new SettingsCommand("privacy policy", "Privacy Policy", OpenPrivacyPolicy);
            args.Request.ApplicationCommands.Add(_privacy);

            var _useridpage = new SettingsCommand("userid", "User Name", OpenUserIDPage);
            args.Request.ApplicationCommands.Add(_useridpage);
        }

        private void OpenUserIDPage(IUICommand command)
        {
            string _userid = APIMASHGlobals.Instance.UserID;

            m_useridPopup = new Popup();
            m_useridPopup.Closed += UseridPopup_Closed;
            Window.Current.Activated += OnUserIdPanelActivated;

            m_useridPopup.IsLightDismissEnabled = true;
            m_useridPopup.Width = m_CharmsFlyoutWidth;
            m_useridPopup.Height = m_windowBounds.Height;

            UserIdUserControl _mypane = new UserIdUserControl();
            _mypane.Width = m_CharmsFlyoutWidth;
            _mypane.Height = m_windowBounds.Height;

            m_useridPopup.Child = _mypane;
            m_useridPopup.SetValue(Canvas.LeftProperty, m_windowBounds.Width - m_CharmsFlyoutWidth);
            m_useridPopup.SetValue(Canvas.TopProperty, 0);
            m_useridPopup.IsOpen = true;
        }

        private void OnUserIdPanelActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                m_useridPopup.IsOpen = false;
            }
        }

        private void UseridPopup_Closed(object sender, object e)
        {
            Window.Current.Activated -= OnUserIdPanelActivated;
        }

        private async void OpenPrivacyPolicy(IUICommand command)
        {
            Uri _uri = new Uri(@"http://blogs.msdn.com/b/jimoneil/archive/2013/02/05/set-up-your-windows-8-privacy-policy-in-five-minutes-or-less.aspx");
            await Launcher.LaunchUriAsync(_uri);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // not implemented
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void Button_FindNearbyPlaces_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FindNearbyPlacesPage));
        }

        private void Button_GeonamesInfo_Click(object sender, RoutedEventArgs e)
        {
            APIMASH_WikiPedia_StarterKit.Common.MessageDialogHelper.ShowMsg("patience grasshopper", 
                "this function isn't available yet, get to work @devfish");
        }

        private void Button_TextSearch_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TextSearchPage));
        }

        private void Button_BoundingBox_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BoundingBoxPage));
        }
    }
}








