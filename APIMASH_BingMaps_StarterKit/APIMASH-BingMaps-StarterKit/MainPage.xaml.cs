using APIMASH_BingMaps_StarterKit.Flyouts;
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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_BingMaps_StarterKit
{
    public sealed partial class MainPage : LayoutAwarePage
    {
        private CurrentLocationPin _currentLocationPin;
        private Geolocator _geolocator;

        public MainPage()
        {
            this.InitializeComponent();

            // check to see if this is the first time application is being executed by check to see if there is any
            // data in local storage. After checking we'll add some notional data.  This will be used to determine whether
            // to prompt (or not) user that location services are not turned on for the app/system.
            Boolean _firstRun = ApplicationData.Current.LocalSettings.Values.Count == 0;
            if (_firstRun)
                ApplicationData.Current.LocalSettings.Values.Add(
                    new System.Collections.Generic.KeyValuePair<string, object>("InitialRunDate", DateTime.UtcNow.ToString()));

            // add a pin for the current location
            _currentLocationPin = new CurrentLocationPin() { Visibility = Visibility.Collapsed };
            MapLayer.SetPositionAnchor(_currentLocationPin, _currentLocationPin.AnchorPoint);
            TheMap.Children.Add(_currentLocationPin);

            // add a layer to store the points of interest
            TheMap.Children.Add(new MapLayer());

            // if location access if revoked while app running, hide the user's location
            _geolocator = new Geolocator();
            _geolocator.StatusChanged += (s, a) =>
                {
                    this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                        new Windows.UI.Core.DispatchedHandler( () =>
                            {
                                if (a.Status == PositionStatus.Disabled)
                                    _currentLocationPin.Visibility = Visibility.Collapsed;
                            })
                    );
                };
            GotoLocation(null, true, !_firstRun);

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
            SearchFlyout.LocationChanged += (s, e) => GotoLocation(new Location(e.Latitude, e.Longitude), showMarker: true);


            // TODO: Implement change in map when an item in the left panel is selected
            LeftPanel.ItemSelected += (s, e) =>
            {
                // do something when item in panel is selected
            };
        }

        /// <summary>
        /// Navigates map centering on given location
        /// </summary>
        /// <param name="location">Latitude/longitude point of new location (if null, current location is detected via GPS)</param>
        /// <param name="ShowMessage">Whether to show message informing user that location tracking is not enabled on device.</param>
        /// <returns></returns>
        async Task GotoLocation(Location location, Boolean showMarker, Boolean ShowMessage = false)
        {
            try
            {
                // a null location is the cue to use geopositioning
                if (location == null)
                {
                    Geoposition currentPosition = await _geolocator.GetGeopositionAsync();
                    location = new Location(currentPosition.Coordinate.Latitude, currentPosition.Coordinate.Longitude);
                }

                // pan map to desired location with a default zoom level
                TheMap.SetView(location, (Double)App.Current.Resources["DefaultZoomLevel"]);

                // refresh the left panel to reflect points of interest in current view
                await LeftPanel.Refresh();

                // set the current position
                MapLayer.SetPosition(_currentLocationPin, location);
                _currentLocationPin.Visibility = showMarker ? Visibility.Visible : Visibility.Collapsed;
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
            // refresh the panel to reflect items in map view
            BottomAppBar.IsOpen = false;
            await LeftPanel.Refresh();
        }        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Modify the contents of the "Flyout" UserObjects to include text or other UI specific to your app
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
    }
}