using APIMASH.Mapping;
using APIMASH_StarterKit.Common;
using APIMASH_StarterKit.Flyouts;
using APIMASH_StarterKit.Mapping;
using Bing.Maps;
using Callisto.Controls;
using Callisto.Controls.Common;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_StarterKit
{
    public sealed partial class MainPage : LayoutAwarePage
    {
        Geolocator _geolocator = new Geolocator();
        CurrentLocationPin _locationMarker = new CurrentLocationPin();

        Boolean _firstRun;

        public MainPage()
        {
            this.InitializeComponent();

            // check to see if this is the first time application is being executed by checking for data in local settings.
            // After checking add some notional data as marker for next time app is run. This will be used to determine whether
            // to prompt the user (or not) that location services are not turned on for the app/system. Without this check, the
            // first time the app is run, it will provide a system prompt, and if that prompt is dismissed without granting 
            // access the propmpt displayed by the application would also appear unnecessarily.
            _firstRun = ApplicationData.Current.LocalSettings.Values.Count == 0;
            if (_firstRun)
                ApplicationData.Current.LocalSettings.Values.Add(
                    new System.Collections.Generic.KeyValuePair<string, object>("InitialRunDate", DateTime.UtcNow.ToString()));

            // register callback to navigate to new spot on map as selected on the SearchFlyout
            SearchFlyout.LocationChanged += (s, e) =>
                {
                    GotoLocation(e.Position);
                    SearchFlyout.Visibility = Visibility.Collapsed;
                };

            // register callback to reset (hide) the user's location, if location access is revoked while app is running
            _geolocator.StatusChanged += (s, a) =>
                {
                    this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                        new Windows.UI.Core.DispatchedHandler(() =>
                            {
                                if (a.Status == PositionStatus.Disabled)
                                    _locationMarker.Visibility = Visibility.Collapsed;
                            })
                    );
                };

            // manage SearchFlyout visibility/interaction
            this.Tapped += (s, e) =>
            {
                if (SearchFlyout.Visibility == Visibility.Visible)
                {
                    SearchFlyout.Visibility = Visibility.Collapsed;
                    e.Handled = true;
                }
            };
            BottomAppBar.Opened += (s, e) => { SearchFlyout.Visibility = Visibility.Collapsed; };
            SearchFlyout.Tapped += (s, e) => { e.Handled = true; };
        }

        /// <summary>
        /// Navigates map centering on given location
        /// </summary>
        /// <param name="position">Latitude/longitude point of new location (if null, current location is detected via GPS)</param>
        /// <param name="showMessage">Whether to show message informing user that location tracking is not enabled on device.</param>
        async Task GotoLocation(LatLong position, Boolean showMessage = false)
        {
            try
            {
                // a null location is the cue to use geopositioning
                if (position == null)
                {
                    try
                    {
                        Geoposition currentPosition = await _geolocator.GetGeopositionAsync();
                        position = new LatLong(currentPosition.Coordinate.Latitude, currentPosition.Coordinate.Longitude);
                    }
                    catch (Exception)
                    {
                        if (showMessage)
                        {
                            MessageDialog md =
                                new MessageDialog("This application is not able to determine your current location. This can occur if your machine is operating in Airplane mode or if the GPS sensor is otherwise not operating.");
                            md.ShowAsync();
                        }
                    }
                }

                // don't assume a valid location at this point GPS/Wifi disabled may lead to a null location
                if (position != null)
                {
                    // move pin to the current location
                    TheMap.SetCurrentLocationPin(_locationMarker, position);

                    // pan map to desired location with a default zoom level
                    TheMap.SetView(new Location(position.Latitude, position.Longitude), (Double)App.Current.Resources["DefaultZoomLevel"]);

                    // refresh the left panel to reflect points of interest in current view
                    await LeftPanel.Refresh(TheMap.TargetBounds.North, TheMap.TargetBounds.South,
                                            TheMap.TargetBounds.West, TheMap.TargetBounds.East);
                }
            }

            // catch exception if location permission not granted
            catch (UnauthorizedAccessException)
            {
                if (showMessage)
                {
                    MessageDialog md =
                        new MessageDialog("This application has not been granted permission to capture your current location. Use the Settings charm to provide this access, then try the operation again.");
                    md.ShowAsync();
                }
            }
        }

        /// <summary>
        /// Occurs when opening the settings pan. Listening for this event lets the app initialize the setting commands and pause its UI until the user closes the pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void settingsPane_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs e)
        {
            //
            // TODO: Modify the contents of the Flyout classes and add/remove other flyouts as required by your application
            //
            e.Request.ApplicationCommands.Add(
                new SettingsCommand("About", "About",
                                    (x) => ShowSettingsFlyout("About", new AboutFlyout()))
            );
            e.Request.ApplicationCommands.Add(
                new SettingsCommand("Support", "Support",
                                    (x) => ShowSettingsFlyout("Support", new SupportFlyout()))
            );
            e.Request.ApplicationCommands.Add(
                new SettingsCommand("Privacy", "Privacy Statement",
                                    (x) => ShowSettingsFlyout("Privacy Statement", new PrivacyFlyout()))
            );
        }

        /// <summary>
        /// Show a settings flyout using the Callisto toolkit (http://callistotoolkit.com/)
        /// </summary>
        /// <param name="title">Name of flyout</param>
        /// <param name="content">UserControl containing the content to be displayed in the flyout</param>
        /// <param name="width">Flyout width (narrow or wide)</param>
        private async void ShowSettingsFlyout(string title, Windows.UI.Xaml.Controls.UserControl content,
            SettingsFlyout.SettingsFlyoutWidth width = SettingsFlyout.SettingsFlyoutWidth.Narrow)
        {
            // grab app theme color from resources (optional)
            SolidColorBrush color = null;
            if (App.Current.Resources.Keys.Contains("AppThemeBrush"))
                color = App.Current.Resources["AppThemeBrush"] as SolidColorBrush;

            // create the flyout
            var flyout = new SettingsFlyout();
            if (color != null) flyout.HeaderBrush = color;
            flyout.HeaderText = title;
            flyout.FlyoutWidth = width;

            // access the small logo from the manifest
            flyout.SmallLogoImageSource = new BitmapImage((await AppManifestHelper.GetManifestVisualElementsAsync()).SmallLogoUri);

            // assign content and show
            flyout.Content = content;
            flyout.IsOpen = true;
        }

        /// <summary>
        /// Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data include the Parameter provided to the pending navigation.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // The BingMaps API allows use of a "session key" if the application leverages the Bing Maps control. By using the session
            // key instead of the API key, only one transaction is logged agains the key versus one transaction for every API call! This 
            // code sets the key asynchronously and stored it as a resource so it's available when the REST API's are invoked.
            if (!Application.Current.Resources.ContainsKey("BingMapsSessionKey"))
                Application.Current.Resources.Add("BingMapsSessionKey", await TheMap.GetSessionIdAsync());

            // if a location was passed as a navigation argument (e.g., from the search page) go directly to that location
            // (if no/invalid parameter, it's a cue for going to current location
            GotoLocation(e.Parameter as LatLong, showMessage: !_firstRun);

            // set up settings flyouts
            SettingsPane settingsPane = Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested += settingsPane_CommandsRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // unregister flyout callback
            SettingsPane settingsPane = Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested -= settingsPane_CommandsRequested;
        }
 
        #region AppBar implementations
        private void FindButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // open the search flyout
            BottomAppBar.IsOpen = false;
            SearchFlyout.Visibility = Visibility.Visible;
        }

        private async void LocationButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // pan map to the user's curent location
            BottomAppBar.IsOpen = false;
            await GotoLocation(null);
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await LeftPanel.Refresh(TheMap.TargetBounds.North, TheMap.TargetBounds.South,
                                    TheMap.TargetBounds.West, TheMap.TargetBounds.East);
        }

        private void Aerial_Click(object sender, RoutedEventArgs e)
        {
            TheMap.MapType = (((ToggleButton)sender).IsChecked ?? false) ? MapType.Aerial : MapType.Road;
        }

        private void Traffic_Click(object sender, RoutedEventArgs e)
        {
            TheMap.ShowTraffic = ((ToggleButton)sender).IsChecked ?? false;
        }
        #endregion
    }
}