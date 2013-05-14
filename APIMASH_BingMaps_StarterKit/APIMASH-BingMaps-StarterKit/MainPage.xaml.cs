using APIMASH_BingMaps;
using Bing.Maps;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_BingMaps_StarterKit
{
    public sealed partial class MainPage : LayoutAwarePage
    {
        private UIElement  _currentLocationPin;
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


            // create a pin to mark user's current location (if granted access)
            //currentLocationPin = Resources["CurrentLocationPin"] as Windows.UI.Xaml.Shapes.Rectangle;

            _currentLocationPin = new Windows.UI.Xaml.Shapes.Rectangle()
            {
                Height = 15,
                Width = 15,
                StrokeThickness = 3,
                Stroke = new SolidColorBrush(Colors.Black),
                Fill = new SolidColorBrush(Colors.Red),
                Visibility = Visibility.Collapsed,
                RenderTransform = new RotateTransform() { Angle = 45 }
            };
            

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
            GotoLocation(null, !_firstRun);

            this.Tapped += (s, e) =>
                {
                    if (SearchFlyout.Visibility == Visibility.Visible)
                    {
                        SearchFlyout.Visibility = Visibility.Collapsed;
                        e.Handled = true;
                    }
                };

            SearchFlyout.Tapped += (s, e) => { e.Handled = true; };
            SearchFlyout.LocationChanged += (s, e) => GotoLocation(new Location(e.Latitude, e.Longitude));

            BottomAppBar.Opened += (s, e) => { SearchFlyout.Visibility = Visibility.Collapsed; };
        }
        
        /// <summary>
        /// Navigates map centering on given location
        /// </summary>
        /// <param name="location">Latitude/longitude point of new location (if null, current location is detected via GPS)</param>
        /// <param name="ShowMessage">Whether to show message informing user that location tracking is not enabled on device.</param>
        /// <returns></returns>
        async Task GotoLocation(Location location, Boolean ShowMessage = false)
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

                // show the traffic cameras currently in map view
                await TomTomPanel.Refresh();

                // add pushpin for current location
                if (!TheMap.Children.Contains(_currentLocationPin))
                    TheMap.Children.Add(_currentLocationPin);
                MapLayer.SetPosition(_currentLocationPin, location);
                _currentLocationPin.Visibility = Visibility.Visible;

            }

            // catch exception is location permission not granted
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
            await GotoLocation(null, true);
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            // refresh the panel to reflect items in map view
            BottomAppBar.IsOpen = false;
            await TomTomPanel.Refresh();
        }        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // handle Settings pane options
            SettingsPane settingsPane = Windows.UI.ApplicationSettings.SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested += (s, e2) =>
            {
                e2.Request.ApplicationCommands.Add(
                    new SettingsCommand("About", "About",
                                        (x) => Launcher.LaunchUriAsync(new Uri("ms-appx:///Assets/About.html")))
                );
                e2.Request.ApplicationCommands.Add(
                    new SettingsCommand("Support", "Support",
                                        (x) => Launcher.LaunchUriAsync(new Uri("ms-appx:///Assets/Support.html")))
                );
                e2.Request.ApplicationCommands.Add(
                    new SettingsCommand("Privacy", "Privacy",
                                        (x) => Launcher.LaunchUriAsync(new Uri("ms-appx:///Assets/Privacy.html")))
                );
            };
        }
    }
}