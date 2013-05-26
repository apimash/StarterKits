using APIMASH;
using APIMASH.Mapping;
using APIMASH_BingMaps;
using APIMASH_StarterKit.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_StarterKit
{
    /// <summary>
    /// LocationSelected event arguments
    /// </summary>
    public class LocationSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Position of selected location
        /// </summary>
        public LatLong Position { get; private set; }

        public LocationSelectedEventArgs(Double latitude, Double longitude)
        {
            this.Position = new LatLong(latitude, longitude);
        }

        public LocationSelectedEventArgs(LatLong position)
        {
            this.Position = position;
        }
    }

    /// <summary>
    /// Flyout panel providing map search feature
    /// </summary>
    public sealed partial class SearchPanel : LayoutAwarePanel
    {
        #region MaxResults dependency property
        public Int32 MaxResults
        {
            get { return (Int32)GetValue(MaxResultsProperty); }
            set { SetValue(MaxResultsProperty, value); }
        }
        public static readonly DependencyProperty MaxResultsProperty =
            DependencyProperty.Register("MaxResults", typeof(Int32), typeof(SearchPanel), new PropertyMetadata(0));
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

        private BingMapsApi _bingMapsApi = new BingMapsApi();
        public SearchPanel()
        {
            this.InitializeComponent();

            // intialize geyneric element of the view model
            this.DefaultViewModel["ApiStatus"] = ApiResponseStatus.DefaultInstance;
            this.DefaultViewModel["SearchResults"] = _bingMapsApi.BingMapsViewModel.Results;
            this.DefaultViewModel["NoResults"] = false;

            // event callback implementation for dismissing the error panel
            ErrorPanel.Dismissed += (s, e) => this.DefaultViewModel["ApiStatus"] = ApiResponseStatus.DefaultInstance;
        }

        // handles the search request
        private async void FindButton_Tapped(object sender, RoutedEventArgs e)
        {
            this.DefaultViewModel["ApiStatus"] = await _bingMapsApi.GetLocations(LocationSearchText.Text, this.MaxResults);
            this.DefaultViewModel["NoResults"] = _bingMapsApi.BingMapsViewModel.Results.Count == 0;
        }

        // triggers when a location has been selected from among the results
        private void LocationList_ItemClick(object sender, ItemClickEventArgs e)
        {
            BingMapsLocationViewModel location = e.ClickedItem as BingMapsLocationViewModel;
            if (location != null)
            {
                OnLocationChanged(new LocationSelectedEventArgs(location.Position));
            }
        }

        // handles the RETURN key as request to initiate search (as long as text box is not empty)
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
