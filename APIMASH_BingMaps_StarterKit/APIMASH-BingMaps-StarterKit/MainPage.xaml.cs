using APIMASH_TomTom;
using Bing.Maps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.System;
using Windows.UI;
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
        private TomTomApi _TomTomApi = new TomTomApi();
        private UIElement _currentLocationPin;
        private Geolocator _geolocator;

        public MainPage()
        {
            this.InitializeComponent();


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
            GotoLocation(null, true);

            this.Tapped += (s, e) =>
                {
                    if (LocateFlyout.Visibility == Visibility.Visible)
                    {
                        LocateFlyout.Visibility = Visibility.Collapsed;
                        e.Handled = true;
                    }
                };

            LocateFlyout.Tapped += (s, e) => { e.Handled = true; };

            LocateFlyout.LocationChanged += (s, e) => GotoLocation(new Location(e.Latitude, e.Longitude));

            BottomAppBar.Opened += (s, e) => { LocateFlyout.Visibility = Visibility.Collapsed; };
        }
        
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
                await PlotCameras();

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
                    MessageDialog md = new MessageDialog("This application has not been granted permission to capture your current location. Use the Settings charm to provide this access, then try the operation again.");
                    md.ShowAsync();
                }
            }
        }

        public async Task PlotCameras()
        {
            // make call to TomTom API to get the camera in the map view
            await TomTomApi.GetCameras(new BoundingBox(
                        TheMap.TargetBounds.North, TheMap.TargetBounds.South,
                        TheMap.TargetBounds.West, TheMap.TargetBounds.East));

            // replace all of the push pins
            TheMap.Children.Clear();
            foreach (var c in TomTomApi.Cameras)
            {
                Pushpin p = new Pushpin();
                p.Text = c.Sequence.ToString();
                p.PointerPressed += (s, e) =>
                {
                    GotoLocation(new Location(c.Latitude, c.Longitude));
                };
                MapLayer.SetPosition(p, new Location(c.Latitude, c.Longitude));
                TheMap.Children.Add(p);
            }
        }

        private void FindButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // open the search flyout
            BottomAppBar.IsOpen = false;            
            LocateFlyout.Visibility = Visibility.Visible;
        }

        private async void LocationButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // pan map to the user's curent location
            BottomAppBar.IsOpen = false;
            await GotoLocation(null, true);
        }

        private async void RefreshCams_Click(object sender, RoutedEventArgs e)
        {
            // refresh the camera list to reflect cameras in map view
            BottomAppBar.IsOpen = false;
            await PlotCameras();
        }        
        
        private void CameraList_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            // update camera image source based on selected camera
            APIMASH_TomTom.TomTomCameraViewModel camera = null;
            if (e.AddedItems.Count > 0)
            {
                camera = e.AddedItems[0] as TomTomCameraViewModel;
                if (camera != null)
                {
                    CamImage.Source = TomTomApi.GetCameraImage(camera.CameraId);
                }
            }
            CamImage.Visibility = camera == null ? Visibility.Collapsed : Visibility.Visible;
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