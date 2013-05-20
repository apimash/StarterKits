using APIMASH;
using APIMASH_StarterKit.Mapping;
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
              
        //
        // TODO: change the type parameter to the API class you've written to encapsulate the "mappable" API class
        //       you're using in your application.  Once you change this type, you'll need to make appropriate coding 
        //       changes everywhere there are references to API-specific calls and view model classes - essentially
        //       any code in this file that references _defaultViewModel.ApiClass
        //
        //
        private LeftPanelViewModel<APIMASH_TomTom.TomTomApi> _defaultViewModel = new LeftPanelViewModel<APIMASH_TomTom.TomTomApi>();


        public LeftPanel()
        {
            this.InitializeComponent();
            this.DataContext = _defaultViewModel;

            // reset the error panel
            ErrorPanel.Dismissed += (s, e) => _defaultViewModel.ApiStatus = ApiResponseStatus.DefaultInstance;

            //
            // TODO: change the reference to reflect the property of your API's ViewModel class that is the basis of the
            //       ObservableCollection bound to the list view in this UserControl. You'll also have to modify the 
            //       Binding references in the XAML to reflect the correct properties in your view model that should 
            //       display in the listview.
            //
            _defaultViewModel.ApiClass.Cameras.CollectionChanged += ViewModel_CollectionChanged;
        }

        //
        // TODO: add code that should execute whenever an item is selected from the listview
        //
        //
        /// <summary>
        /// Carries out application-specific handling of the item selected in the listview. The synchronization with
        /// the map display is already accomodated.
        /// </summary>
        /// <param name="item">Newly selected item that should be case to a view model class for further processing</param>
        /// <returns></returns>
        private async Task ProcessSelectedItem(object item)
        {
            await _defaultViewModel.ApiClass.GetCameraImage(item as APIMASH_TomTom.TomTomCameraViewModel);
        }

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
                await _defaultViewModel.ApiClass.GetCameras(new APIMASH_TomTom.BoundingBox(
                        Map.TargetBounds.North, Map.TargetBounds.South,
                        Map.TargetBounds.West, Map.TargetBounds.East),
                        this.MaxResults);
        }

        #region event handlers (API agnostic)
        // handle synchronization for new selection in the list with the map
        private async void MappableListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // get newly selected and deseleced item
            object newItem = e.AddedItems.FirstOrDefault();
            object oldItem = e.RemovedItems.FirstOrDefault();

            // highlight/unhighlight map pins 
            MapUtilities.HighlighPointOfInterestPin(Map, newItem as IMappable, true);
            MapUtilities.HighlighPointOfInterestPin(Map, oldItem as IMappable, false);

            // process new selection
            if (newItem != null)
                await ProcessSelectedItem(newItem);

            // HACK ALERT: explicitly setting visibility because a reset of list isn't triggering the rebinding so 
            // that the XAML converter for Visibility kicks in.  
            MappableItem.Visibility = newItem == null ? Visibility.Collapsed : Visibility.Visible;

            // attach handler to ensure selected item is in view after layout adjustments
            MappableListView.LayoutUpdated += MappableListView_LayoutUpdated;
        }

        // make sure selected item is visible whenever list updates
        void MappableListView_LayoutUpdated(object sender, object e)
        {
            MappableListView.ScrollIntoView(MappableListView.SelectedItem ?? MappableListView.Items.FirstOrDefault());
            MappableListView.LayoutUpdated -= MappableListView_LayoutUpdated;
        }

        // synchronize changes in the view model collection with the map push pins
        void ViewModel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // only additions and wholesale reset of the ObservableCollection are currently supported
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        IMappable mapItem = (IMappable)item;

                        PointOfInterestPin poiPin = new PointOfInterestPin(mapItem);
                        poiPin.Selected += (s, e2) =>
                            {
                                MappableListView.SelectedItem = MappableListView.Items.Where((c) => (c as IMappable).Id == e2.PointOfInterest.Id).FirstOrDefault();
                            };

                        MapUtilities.AddPointOfInterestPin(poiPin, Map, new Location(mapItem.Latitude, mapItem.Longitude));
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

        // invoke refresh when clicking on glyph next to title
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
        #endregion
    }    
    
    /// <summary>
    /// View model supporting functionality of the left panel
    /// </summary>
    /// <typeparam name="T">Type of API being used</typeparam>
    public sealed class LeftPanelViewModel<T> : APIMASH.BindableBase where T : APIMASH.ApiBase, new()
    {
        /// <summary>
        /// API wrapper class (extends APIMASH.ApiBase)
        /// </summary>
        public T ApiClass
        {
            get { return _apiClass; }
            set { SetProperty(ref _apiClass, value); }
        }
        private T _apiClass;

        /// <summary>
        /// API response status
        /// </summary>
        public ApiResponseStatus ApiStatus
        {
            get { return _apiStatus; }
            set { SetProperty (ref _apiStatus, value); }
        }
        private ApiResponseStatus _apiStatus;

        public LeftPanelViewModel()
        {
            ApiClass = new T();
            ApiStatus = ApiResponseStatus.DefaultInstance;
        }
    }
}
