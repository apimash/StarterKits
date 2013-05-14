using APIMASH_BingMaps;
using APIMASH_BingMaps_StarterKit;
using APIMASH_BingMaps_StarterKit.Common;
using Bing.Maps;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_BingMaps_StarterKit.Flyouts
{
    /// <summary>
    /// LocationSelected event arguments
    /// </summary>
    public class LocationSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Latitude of selected location
        /// </summary>
        public Double Latitude { get; private set; }
        /// <summary>
        /// Longitude of selected location
        /// </summary>
        public Double Longitude { get; private set; }

        public LocationSelectedEventArgs(Double lat, Double lon)
        {
            this.Latitude = lat;
            this.Longitude = lon;
        }
    }

    /// <summary>
    /// View model supporting the Search flyout
    /// </summary>
    public sealed class SearchFlyoutViewModel : BindableBase
    {

        private BingApi _bingApi;
        /// <summary>
        /// Bing Maps API wrapper class <seealso cref="APIMASH_BingMaps.BingApi"/>
        /// </summary>
        public BingApi BingApi
        {
            get { return _bingApi; }
            set { SetProperty(ref _bingApi, value); }
        }

        private APIMASH.ApiResponseStatus _apiStatus;
        /// <summary>
        /// API response status
        /// </summary>
        public APIMASH.ApiResponseStatus ApiStatus
        {
            get { return _apiStatus; }
            set { SetProperty (ref _apiStatus, value); }
        }

        public SearchFlyoutViewModel()
        {
            BingApi = new BingApi();
            ApiStatus = APIMASH.ApiResponseStatus.DefaultInstance;
        }
    }

    /// <summary>
    /// Flyout panel providing map search feature
    /// </summary>
    public sealed partial class SearchFlyout : UserControl
    {
        #region Map dependency property
        /// <summary>
        /// Map associated with search flyout
        /// <remarks>The Map is a dependency property and needed only to obtain a session key from the Map control to use in lieu
        /// of the API key, since the session key will result in only one "billable" transaction against the 50K transactions available
        /// with the basic Bing Maps license.  The PropertyChangedCallback will refresh the session key whenever the map is changed, and
        /// that should occur precisely once: when the application starts</remarks>
        /// </summary>
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }
        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register("Map", typeof(Map), typeof(SearchFlyout),
            new PropertyMetadata("", new PropertyChangedCallback(OnMapChanged)));
        public async static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
             ((SearchFlyout) d)._defaultViewModel.BingApi.SetSessionKey(await ((Map)e.NewValue).GetSessionIdAsync());
        }
        #endregion

        #region MaxResults dependency property
        public Int32 MaxResults
        {
            get { return (Int32)GetValue(MaxResultsProperty); }
            set { SetValue(MaxResultsProperty, value); }
        }
        public static readonly DependencyProperty MaxResultsProperty =
            DependencyProperty.Register("MaxResults", typeof(Int32), typeof(SearchFlyout), new PropertyMetadata(0));
        #endregion

        #region LocationSelected event handler
        /// <summary>
        /// Occurs when one of location search results is selected. Attach an event handler here to reflect selection of the location in the main map.
        /// </summary>
        public event EventHandler<LocationSelectedEventArgs> LocationChanged;
        private void OnLocationChanged(LocationSelectedEventArgs e)
        {
            if (LocationChanged != null) LocationChanged(this, e);
        }
        #endregion

        SearchFlyoutViewModel _defaultViewModel = new SearchFlyoutViewModel();
        public SearchFlyout()
        {
            this.InitializeComponent();

            this.DataContext = _defaultViewModel;
            ErrorPanel.Dismissed += (s, e) => _defaultViewModel.ApiStatus = APIMASH.ApiResponseStatus.DefaultInstance;
        }

        // handle the search request
        private async void FindButton_Tapped(object sender, RoutedEventArgs e)
        {
            _defaultViewModel.ApiStatus = await _defaultViewModel.BingApi.GetLocations(LocationSearchText.Text, this.MaxResults);
        }

        // trigger event when a location has been selected from among the results
        private void LocationList_ItemClick(object sender, ItemClickEventArgs e)
        {
            BingLocationsViewModel location = e.ClickedItem as BingLocationsViewModel;
            if (location != null)
            {
                OnLocationChanged(new LocationSelectedEventArgs(location.Latitude, location.Longitude));
            }
        }

        // handle return key as request to initiate search (as long as text box is not empty)
        private void LocationText_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if ((e.Key == Windows.System.VirtualKey.Enter) && (LocationSearchText.Text.Trim().Length > 0))
            {
                e.Handled = true;
                FindButton_Tapped(sender, e);
            }
        }
    }
}
