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

        public MainPage()
        {
            this.InitializeComponent();

            // check to see if this is the first time application is being executed by checking for data in local settings.
            // After checking add some notional data as marker for next time app is run. This will be used to determine whether
            // to prompt the user (or not) that location services are not turned on for the app/system. Without this check, the
            // first time the app is run, it will provide a system prompt, and if that prompt is dismissed without granting 
            // access the propmpt displayed by the application would also appear unnecessarily.
            Boolean _firstRun = ApplicationData.Current.LocalSettings.Values.Count == 0;
            if (_firstRun)
                ApplicationData.Current.LocalSettings.Values.Add(
                    new System.Collections.Generic.KeyValuePair<string, object>("InitialRunDate", DateTime.UtcNow.ToString()));

            // navigate to the user's current location (if so allowed)
            GotoLocation(null, showMarker: true, ShowMessage: !_firstRun);

            // register callback to navigate to new spot on map as selected on the SearchFlyout
            SearchFlyout.LocationChanged += (s, e) => GotoLocation(new Location(e.Latitude, e.Longitude), showMarker: true);

            //
            //
            // TODO: Implement change in map when an item in the left panel is selected
            //
            //
            LeftPanel.ItemSelectionChanged += (s, e) =>
            {
                // do something when item in panel is selected
            };

            // register callback to reset (hide) the user's location, if location access is revoked while app is running
            _geolocator.StatusChanged += (s, a) =>
                {
                    this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                        new Windows.UI.Core.DispatchedHandler( () =>
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
        /// <param name="location">Latitude/longitude point of new location (if null, current location is detected via GPS)</param>
        /// <param name="ShowMessage">Whether to show message informing user that location tracking is not enabled on device.</param>
        async Task GotoLocation(Location location, Boolean showMarker, Boolean ShowMessage = false)
        {
            try
            {
                // a null location is the cue to use geopositioning
                if (location == null)
                {
                    try
                    {
                        Geoposition currentPosition = await _geolocator.GetGeopositionAsync();
                        location = new Location(currentPosition.Coordinate.Latitude, currentPosition.Coordinate.Longitude);
                    }
                    catch (Exception)
                    {
                        MessageDialog md =
                            new MessageDialog("This application is not able to determine your current location. This can occur if your machine is operating in Airplane mode or if the GPS sensor is otherwise not operating.");
                        md.ShowAsync();
                    }
                }

                // don't assume a valid location at this point GPS/Wifi disabled may lead to a null location
                if (location != null)
                {
                    // move pin ot the current location
                    _locationMarker.SetLocation(TheMap, location);

                    // pan map to desired location with a default zoom level
                    TheMap.SetView(location, (Double)App.Current.Resources["DefaultZoomLevel"]);

                    // refresh the left panel to reflect points of interest in current view
                    await LeftPanel.Refresh();
                }
            }

            // catch exception if location permission not granted
            catch (UnauthorizedAccessException)
            {
                if (ShowMessage)
                {
                    MessageDialog md = 
                        new MessageDialog("This application has not been granted permission to capture your current location. Use the Settings charm to provide this access, then try the operation again.");
                    md.ShowAsync();
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //
            //
            // TODO: Modify the contents of the "Flyout" UserObjects to include text or other UI specific to your app
            //
            //
            SettingsPane settingsPane = Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested += (s, e2) =>
            {
                e2.Request.ApplicationCommands.Add(
                    new SettingsCommand("About", "About",
                                        (x) => ShowSettingsFlyout("About", new AboutFlyout()))
                );
                e2.Request.ApplicationCommands.Add(
                    new SettingsCommand("Support", "Support",
                                        (x) => ShowSettingsFlyout("Support", new SupportFlyout()))
                );
                e2.Request.ApplicationCommands.Add(
                    new SettingsCommand("Privacy", "Privacy Statement",
                                        (x) => ShowSettingsFlyout("Privacy Statement", new PrivacyFlyout()))
                );
            };
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
            if (App.Current.Resources.Keys.Contains("AppThemeColor"))
                 color = App.Current.Resources["AppThemeColor"] as SolidColorBrush;

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
            await GotoLocation(null, showMarker: true);
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await LeftPanel.Refresh();
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