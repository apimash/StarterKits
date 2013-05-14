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
    
        LeftPanelViewModel _defaultViewModel = new LeftPanelViewModel();
        public LeftPanel()
        {
            this.InitializeComponent();

            this.DataContext = _defaultViewModel;
            ErrorPanel.Dismissed += (s, e) => _defaultViewModel.ApiStatus = APIMASH.ApiResponseStatus.DefaultInstance;
        }

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
            }
            CamImage.Visibility = camera == null ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        // handle refresh request of the data in the left panel
        public async Task Refresh()
        {
            // make call to TomTom API to get the camera in the map view
            _defaultViewModel.ApiStatus = 
                await _defaultViewModel.TomTomApi.GetCameras(new BoundingBox(
                        Map.TargetBounds.North, Map.TargetBounds.South,
                        Map.TargetBounds.West, Map.TargetBounds.East));

            // replace all of the push pins
            Map.Children.Clear();
            foreach (var c in _defaultViewModel.TomTomApi.Cameras)
            {
                Pushpin p = new Pushpin();
                p.Text = c.Sequence.ToString();
                p.PointerPressed += (s, e) =>
                {
                    ///GotoLocation(new Location(c.Latitude, c.Longitude));
                };
                MapLayer.SetPosition(p, new Location(c.Latitude, c.Longitude));
                Map.Children.Add(p);
            }

            var item = CameraListView.Items.Where(o => (o as TomTomCameraViewModel).Sequence == 1).FirstOrDefault();
            if (item != null)
                CameraListView.ScrollIntoView(
                    item, ScrollIntoViewAlignment.Leading
                );
        }
    }
}
