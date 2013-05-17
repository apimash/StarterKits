using APIMASH;
using APIMASH_StarterKit.Mapping;
using APIMASH_TomTom;
using Bing.Maps;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_StarterKit
{

    /// <summary>
    /// ItemSelectionChanged event arguments
    /// </summary>
    public class ItemSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Newly selected item
        /// </summary>
        public Object NewItem { get; private set; }


        /// <summary>
        /// Previously selected item (or null)
        /// </summary>
        public Object OldItem { get; private set; }

        public ItemSelectionChangedEventArgs(Object n, Object o)
        {
            NewItem = n;
            OldItem = o;
        }
    }

    /// <summary>
    /// View model supporting left panel of items associated with map
    /// </summary>
    public sealed class LeftPanelViewModel : APIMASH.BindableBase
    {
        /// <summary>
        /// TomTom API wrapper class
        /// </summary>
        public TomTomApi TomTomApi
        {
            get { return _tomTomApi; }
            set { SetProperty(ref _tomTomApi, value); }
        }
        private TomTomApi _tomTomApi;

        /// <summary>
        /// API response status
        /// </summary>
        public APIMASH.ApiResponseStatus ApiStatus
        {
            get { return _apiStatus; }
            set { SetProperty (ref _apiStatus, value); }
        }
        private APIMASH.ApiResponseStatus _apiStatus;

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
        public event EventHandler<ItemSelectionChangedEventArgs> ItemSelectionChanged;
        private void OnItemSelectionChanged(ItemSelectionChangedEventArgs e)
        {            
            MapUtilities.HighlighPointOfInterestPin(Map, e.NewItem as IMappable, true);
            MapUtilities.HighlighPointOfInterestPin(Map, e.OldItem as IMappable, false);


            if (ItemSelectionChanged != null) ItemSelectionChanged(this, e);
        }
        #endregion
       
        LeftPanelViewModel _defaultViewModel = new LeftPanelViewModel();
        public LeftPanel()
        {
            this.InitializeComponent();
            this.DataContext = _defaultViewModel;

            // use collection changed event to associate view model with point-of-interest pin on map
            _defaultViewModel.TomTomApi.Cameras.CollectionChanged += ViewModel_CollectionChanged;

            // reset the error panel
            ErrorPanel.Dismissed += (s, e) => _defaultViewModel.ApiStatus = APIMASH.ApiResponseStatus.DefaultInstance;
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object newItem = e.AddedItems.FirstOrDefault();
            object oldItem = e.RemovedItems.FirstOrDefault();

            //
            //
            // TODO: handle the selection of an INDIVIDUAL item in the list. Note that the main view can listen 
            //       to changes via the ItemSelectedEvent.  If you enable multi-selection on the list, this 
            //       code will need to be updated accordingly.
            //
            //
            
            if (newItem != null)
            {
                _defaultViewModel.TomTomApi.GetCameraImage(newItem as TomTomCameraViewModel);
            }



            // trigger event listeners
            OnItemSelectionChanged(new ItemSelectionChangedEventArgs(newItem, oldItem));
        }

        //
        //
        // TODO: refresh the items in the panel to reflect points of interest in the current map view.
        //       this will also require modifying the XAML bindings to reflect properties and information
        //       you want expose from your specific view model
        //
        //
        /// <summary>
        /// Refresh the list of items obtained from the API and have it populate the view model
        /// </summary>
        public async Task Refresh()
        {
            // make call to TomTom API to get the cameras in the map view
            _defaultViewModel.ApiStatus = 
                await _defaultViewModel.TomTomApi.GetCameras(new BoundingBox(
                        Map.TargetBounds.North, Map.TargetBounds.South,
                        Map.TargetBounds.West, Map.TargetBounds.East),
                        this.MaxResults);

            // scroll list back to top
            if (CameraListView.Items.Count > 0)
                CameraListView.ScrollIntoView(CameraListView.Items[0]);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        // synchronizes changes in the view model collection with the map push pins
        void ViewModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // only additions and wholesale reset of the ObservableCollection are currently supported
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        IMappable mapItem = (IMappable)item;
                        MapUtilities.AddPointOfInterestPin(
                            new PointOfInterestPin(mapItem.Id, mapItem.Label),
                            Map,
                            new Location(mapItem.Latitude, mapItem.Longitude)
                        );
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    MapUtilities.ClearPointOfInterestPins(Map);
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
    }
}
