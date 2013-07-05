using APIMASH.Mapping;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Specialized;
using System.Device.Location;
using System.Threading.Tasks;
using System.Windows;
using TomTom_StarterKit_Phone.Common;
using TomTom_StarterKit_Phone.Mapping;
using TomTom_StarterKit_Phone.Resources;
using Windows.Devices.Geolocation;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace TomTom_StarterKit_Phone
{
    //
    // TODO: Set the appropriate data types for the ApiViewModel and SelectedItem properties
    //       in the MainPageViewModel below to reflect the elements of your API's ViewModel
    //

    /// <summary>
    /// View model used by main page
    /// </summary>
    public class MainPageViewModel : BindableBase
    {
        /// <summary>
        /// Status structure returned from each call to an API
        /// </summary>
        public APIMASH.ApiResponseStatus ApiStatus {
            get { return _apiStatus; }
            set { SetProperty(ref _apiStatus, value); }
        }
        private APIMASH.ApiResponseStatus _apiStatus;       
        
        /// <summary>
        /// View model structure managed by API library
        /// </summary>
        public APIMASH_TomTom.TomTomViewModel ApiViewModel { 
            get { return _apiViewModel; }
            set { SetProperty(ref _apiViewModel, value); }
        }
        private APIMASH_TomTom.TomTomViewModel _apiViewModel;

        /// <summary>
        /// Contains selected item within the list of items surfaced by the API
        /// </summary>
        public APIMASH_TomTom.TomTomCameraViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }
        private APIMASH_TomTom.TomTomCameraViewModel _selectedItem;
    }

    public partial class MainPage : PhoneApplicationPage
    {
        CurrentLocationPin _locationMarker = new CurrentLocationPin();
        Geolocator _geolocator;

        // view models
        public MainPageViewModel DefaultViewModel { get; set; }        
        SettingsViewModel _settingsViewModel;
        APIMASH.ApiMonitor _apiMonitor;

        // API class reference
        APIMASH_TomTom.TomTomApi _tomTomApi = new APIMASH_TomTom.TomTomApi();
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // set up page's view model
            this.DefaultViewModel = new MainPageViewModel() { };
            this.DataContext = this.DefaultViewModel;

            // handle callback from change to ViewModel ObservableCollection to synchronize with map pins
            this.DefaultViewModel.ApiViewModel = _tomTomApi.TomTomViewModel;
            _tomTomApi.TomTomViewModel.Results.CollectionChanged += Results_CollectionChanged;

            // tap into view model containing settings data
            _settingsViewModel = App.Current.Resources["SettingsViewModel"] as SettingsViewModel;

            // get reference to APIMonitor for handling progress bar state
            _apiMonitor = App.Current.Resources["ApiMonitor"] as APIMASH.ApiMonitor;

            // initialize map when page is done loading
            this.Loaded += Map_LoadedForFirstTime;  
        }

        /// <summary>
        /// Handles loading map and navigating to current location but only when map first
        /// loaded on initial navigation to this page.
        /// </summary>
        async void Map_LoadedForFirstTime(object sender, RoutedEventArgs e)
        {
            // do this only once in lifetime of app
            this.Loaded -= Map_LoadedForFirstTime;            

            // go to the current location
            await GotoCurrentLocation();
        }

        /// <summary>
        /// Handles navigation to the given page
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // update map to reflect potential setting change (XAML binding not working on ColorMode, so do programmatically)
            TheMap.ColorMode = _settingsViewModel.UseLightMode ? MapColorMode.Light : MapColorMode.Dark;
            if (!_settingsViewModel.UseLocation || !_settingsViewModel.LocationEnabled) _locationMarker.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Pans map to the current location, possibly prompting user to reset application level setting if he/she has turned off location services
        /// </summary>
        public async Task GotoCurrentLocation()
        {
            // if location services turned off at application level, give user option to reset them
            if (!_settingsViewModel.UseLocation)
            {
                _settingsViewModel.UseLocation = MessageBox.Show("You have turned off location services for this application. If you would like to renable them, press OK; otherwise, press Cancel",
                     "My Location feature unavailable",
                     MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            }

            // navigate to current location
            if (_settingsViewModel.UseLocation)
            {
                await GotoLocation(null);
            }
        }

        /// <summary>
        /// Pans map to given location and queries for cameras now in view
        /// </summary>
        /// <param name="position">Position to which to pan the map (null indicates current location)</param>
        async Task GotoLocation(GeoCoordinate position)
        {
            Boolean currentLocationRequested = position == null;

            // turn on progress bar
            if (_apiMonitor != null) _apiMonitor.IsExecuting = true;
            DefaultViewModel.SelectedItem = null;

            // a null location is the cue to use geopositioning
            if (currentLocationRequested)
            {
                try
                {
                    _geolocator = new Geolocator();
  
#if DEBUG
                    // location emulator will always set current location to Redmond unless accuracy is set to High
                    _geolocator.DesiredAccuracy = PositionAccuracy.High;
#endif

                    // have to set this (or MovementThreshold) in WP8 before attaching event handlers
                    _geolocator.ReportInterval = uint.MaxValue;

                    // register callback to reset (hide) the user's location if location access is revoked while app is running
                    _geolocator.StatusChanged += (s, a) =>
                    {
                        this.Dispatcher.BeginInvoke(
                            new Windows.UI.Core.DispatchedHandler(() =>
                            {
                                if (a.Status == PositionStatus.Disabled)
                                {
                                    _settingsViewModel.LocationEnabled = false;
                                    _locationMarker.Visibility = Visibility.Collapsed;
                                }
                            })
                        );
                    };

                    // get current location
                    Geoposition currentPosition = await _geolocator.GetGeopositionAsync();
                    position = currentPosition.Coordinate.ToGeoCoordinate();
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        String.Format("Location services are turned off for this device, so {0} cannot plot your current position. You can, however, still search for and view camera images within the application.", AppResources.ApplicationTitle),
                        "Location services are off",
                        MessageBoxButton.OK);
                }
            }

            // don't assume a valid location at this point GPS/Wifi disabled may lead to a null location
            if (position != null)
            {
                // register event handler to do work once the view has been reset
                TheMap.ViewChanged += TheMap_ViewChanged;

                // set pin for current location
                if (currentLocationRequested) TheMap.SetCurrentLocationPin(_locationMarker, position);

                // pan map to desired location with a default zoom level (when complete, TheMap_ViewChanged event will fire)
                TheMap.SetView(position, (Double)App.Current.Resources["DefaultZoomLevel"]);
            }

            // turn off progress bar
            if (_apiMonitor != null) _apiMonitor.IsExecuting = false;
        }
        
        /// <summary>
        /// Fires when map animation is complete following a call to SetView. This does NOT fire when the user zooms or pans manually.
        /// </summary>
        async void TheMap_ViewChanged(object sender, MapViewChangedEventArgs e)
        {
            // reset progress indicators
            if (_apiMonitor != null) _apiMonitor.IsExecuting = false;

            // unregister the handler
            TheMap.ViewChanged -= TheMap_ViewChanged;

            // refresh the map
            await Refresh();
        }

        /// <summary>
        /// Refreshes the list of items obtained from the API and populates the view model
        /// </summary>
        public async Task Refresh()
        {
            // get bounding box of current map
            var topLeft = TheMap.ConvertViewportPointToGeoCoordinate(new Point(0, 0));
            var bottomRight = TheMap.ConvertViewportPointToGeoCoordinate(new Point(TheMap.ActualWidth, TheMap.ActualHeight));
            var box = new BoundingBox(topLeft.Latitude, bottomRight.Latitude, topLeft.Longitude, bottomRight.Longitude);

            //
            // TODO: invoke API that returns a list of point-of-interest items now in view on the map. This will populate
            //       an observable collection of IMappable items on the ApiViewModel class (with the name Results). As that 
            //       collection is populated, the Results_CollectionChanged callback will fire to create the appropriate
            //       point-of-interest pins and associate them with the map.
            //
            this.DefaultViewModel.ApiStatus = await _tomTomApi.GetCameras(box, (Int32)App.Current.Resources["MaxResults"]);

            // if there's a problem in the request, show a message box
            if (!DefaultViewModel.ApiStatus.IsSuccessStatusCode)
            {
                MessageBox.Show(
                    String.Format("There was an error in handling the last request.\n\nCode: {0}\n\nMessage: {1}", 
                                               (Int32) DefaultViewModel.ApiStatus.StatusCode, 
                                               String.IsNullOrEmpty(DefaultViewModel.ApiStatus.Message) ?  DefaultViewModel.ApiStatus.StatusCode.ToString() :  DefaultViewModel.ApiStatus.Message),
                    "Request Error",
                    MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Handles selection of a given point-of-interest in response
        /// </summary>
        /// <param name="item">Selected item (typically of a ViewModel class)</param>
        /// <returns></returns>
        private async Task ProcessSelectedItem(object item)
        {            
            //
            // TODO: replace with code that should execute whenever an point-of-interest item is selected from the map
            //
            DefaultViewModel.SelectedItem = item as APIMASH_TomTom.TomTomCameraViewModel;
            await _tomTomApi.GetCameraImage(DefaultViewModel.SelectedItem);

            //
            // TODO: (optional) handle errors from requesting a specific point-of-interest item. In this particular sample
            //       an image is ALWAYS returned, even if it's one simply containing error text and served up from a 
            //       embedded resource in the APIMASH library
        }

        /// <summary>
        /// Dismiss current item's detail image
        /// </summary>
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TheMap.HighlightPointOfInterestPin(DefaultViewModel.SelectedItem, false);
            DefaultViewModel.SelectedItem = null;
        }

        /// <summary>
        /// Synchronizes changes to the ApiViewModel's Results collection. The elements of that collection are assumed
        /// to implement IMappable (see the APIMASH_TomTom.TomTomCameraViewModel implementation for a sample reference).
        /// This code should require no changed regardless as long as the items in the Results collection implement IMappable.
        /// </summary>
        void Results_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
             // only additions and wholesale reset of the ObservableCollection are currently supported
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        IMappable mapItem = (IMappable)item;

                        PointOfInterestPin poiPin = new PointOfInterestPin(mapItem);
                        poiPin.Tap += async (s, e2) =>
                        {
                            TheMap.HighlightPointOfInterestPin(DefaultViewModel.SelectedItem, false);
                            TheMap.HighlightPointOfInterestPin(poiPin.PointOfInterest, true);

                            await ProcessSelectedItem(item);
                        };

                        TheMap.AddPointOfInterestPin(poiPin, mapItem.Position);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    TheMap.ClearPointOfInterestPins();
                    break;

                // case NotifyCollectionChangedAction.Remove:    
                //
                // TODO: (optional) if your application allows items to be selectively removed from the view model
                //       code to remove a single associated push pin will be required.
                //
                //
                //
                // break;


                // not implemented in this context
                // case NotifyCollectionChangedAction.Replace:
                // case NotifyCollectionChangedAction.Move:
            }
        }

        #region AppBar implementations
        private async void Location_Click(object sender, EventArgs e)
        {
            await GotoCurrentLocation();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private async void Refresh_Click(object sender, EventArgs e)
        {
            await Refresh();
        }
        #endregion

    }
}