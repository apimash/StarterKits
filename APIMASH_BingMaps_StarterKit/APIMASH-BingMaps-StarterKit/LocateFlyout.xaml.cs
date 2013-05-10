using APIMASH_BingMaps;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_BingMaps_StarterKit
{
    public delegate void LocationSelectedEventHandler(object sender, LocationSelectedEventArgs e);

    public class LocationSelectedEventArgs : EventArgs
    {
        public Double Latitude { get; private set; }
        public Double Longitude { get; private set; }

        public LocationSelectedEventArgs(Double lat, Double lon)
        {
            this.Latitude = lat;
            this.Longitude = lon;
        }
    }

    public sealed partial class LocateFlyout : UserControl
    {
        #region LocationSelectedEvent handler
        public event LocationSelectedEventHandler LocationChanged;
        private void OnLocationChanged(LocationSelectedEventArgs e)
        {
            if (LocationChanged != null) LocationChanged(this, e);
        }
        #endregion

        public LocateFlyout()
        {
            this.InitializeComponent();
        }

        private async void FindButton_Tapped(object sender, RoutedEventArgs e)
        {
           await BingApi.GetLocations(LocationSearchText.Text);
        }

        private void LocationList_ItemClick(object sender, ItemClickEventArgs e)
        {
            BingLocationsViewModel location = e.ClickedItem as BingLocationsViewModel;
            if (location != null)
            {
                OnLocationChanged(new LocationSelectedEventArgs(location.Latitude, location.Longitude));
            }
        }

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
