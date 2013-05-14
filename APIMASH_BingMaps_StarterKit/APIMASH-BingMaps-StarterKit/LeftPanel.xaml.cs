using APIMASH_BingMaps_StarterKit.Common;
using APIMASH_TomTom;
using Bing.Maps;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_BingMaps_StarterKit
{

    /// <summary>
    /// ItemSelected event arguments
    /// </summary>
    public class ItemSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Item selected
        /// </summary>
        public Object Item { get; private set; }

        public ItemSelectedEventArgs(Object i)
        {
            Item = i;
        }
    }

    /// <summary>
    /// View model supporting left panel of items associated with map
    /// </summary>
    public sealed class LeftPanelViewModel : BindableBase
    {
        private TomTomApi _tomTomApi;
        /// <summary>
        /// TomTom API wrapper class
        /// </summary>
        public TomTomApi TomTomApi
        {
            get { return _tomTomApi; }
            set { SetProperty(ref _tomTomApi, value); }
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

        public LeftPanelViewModel()
        {
            TomTomApi = new TomTomApi();
            ApiStatus = APIMASH.ApiResponseStatus.DefaultInstance;
        }
    }

    public sealed partial class LeftPanel : UserControl
    {
        #region Map dependency property
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }
        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register("Map", typeof(Map), typeof(LeftPanel), new PropertyMetadata(0));
        #endregion

        #region MaxResults dependency property
        public Int32 MaxResults
        {
            get { return (Int32)GetValue(MaxResultsProperty); }
            set { SetValue(MaxResultsProperty, value); }
        }
        public static readonly DependencyProperty MaxResultsProperty =
            DependencyProperty.Register("MaxResults", typeof(Int32), typeof(LeftPanel), new PropertyMetadata(0));
        #endregion  

        #region ItemSelected event handler
        /// <summary>
        /// Occurs when an item is selected in this panel's list view. Attach an event handler here to reflect selection of this item on the main map.
        /// </summary>
        public event EventHandler<ItemSelectedEventArgs> ItemSelected;
        private void OnItemSelected(ItemSelectedEventArgs e)
        {
            if (ItemSelected != null) ItemSelected(this, e);
        }
        #endregion

        LeftPanelViewModel _defaultViewModel = new LeftPanelViewModel();
        public LeftPanel()
        {
            this.InitializeComponent();

            this.DataContext = _defaultViewModel;
            ErrorPanel.Dismissed += (s, e) => _defaultViewModel.ApiStatus = APIMASH.ApiResponseStatus.DefaultInstance;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        // TODO: handle the selection of an individual item in the list. Note that the main view can listen 
        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // update camera image source based on selected camera
            APIMASH_TomTom.TomTomCameraViewModel camera = null;
            if (e.AddedItems.Count > 0)
            {
                camera = e.AddedItems[0] as TomTomCameraViewModel;
                if (camera != null)
                {
                    CamImage.Source = _defaultViewModel.TomTomApi.GetCameraImage(camera.CameraId);
                }
                OnItemSelected(new ItemSelectedEventArgs(camera));
            }
            CamImage.Visibility = camera == null ? Visibility.Collapsed : Visibility.Visible;
        }

        // TODO: refresh the items in the panel to reflect points of interest in the current map view.
        public async Task Refresh()
        {

            // make call to TomTom API to get the camera in the map view
            _defaultViewModel.ApiStatus = 
                await _defaultViewModel.TomTomApi.GetCameras(new BoundingBox(
                        Map.TargetBounds.North, Map.TargetBounds.South,
                        Map.TargetBounds.West, Map.TargetBounds.East),
                        this.MaxResults);

            // clear the map layer of points - note this assumes the first MapLayer in the map children is the one containing
            // point of interest pushpins
            var _poiLayer = Map.Children.OfType<MapLayer>().FirstOrDefault();
            if (_poiLayer != null)
            {
                _poiLayer.Children.Clear();

                foreach (var c in _defaultViewModel.TomTomApi.Cameras)
                {
                    PointOfInterestPin p = new PointOfInterestPin() { Id = c.Sequence };
                    MapLayer.SetPosition(p, new Location(c.Latitude, c.Longitude));
                    _poiLayer.Children.Add(p);
                }
            }

            var item = CameraListView.Items.Where(o => (o as TomTomCameraViewModel).Sequence == 1).FirstOrDefault();
            if (item != null)
                CameraListView.ScrollIntoView(
                    item, ScrollIntoViewAlignment.Leading
                );
        }
    }
}
