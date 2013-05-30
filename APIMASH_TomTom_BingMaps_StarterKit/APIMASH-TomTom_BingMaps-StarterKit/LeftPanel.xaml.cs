using APIMASH;
using APIMASH.Mapping;
using APIMASH_StarterKit.Common;
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
    public sealed partial class LeftPanel : LayoutAwarePanel
    {
        #region Map dependency property
        /// <summary>
        /// Reference to map on the main page
        /// </summary>
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }
        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register("Map", typeof(Map), typeof(LeftPanel), new PropertyMetadata(0));
        #endregion

        #region MaxResults dependency property
        /// <summary>
        /// Maximum number of results that will appear in panel ListView (0 indicates no limit)
        /// </summary>
        public Int32 MaxResults
        {
            get { return (Int32)GetValue(MaxResultsProperty); }
            set { SetValue(MaxResultsProperty, value); }
        }
        public static readonly DependencyProperty MaxResultsProperty =
            DependencyProperty.Register("MaxResults", typeof(Int32), typeof(LeftPanel), new PropertyMetadata(0));
        #endregion

        //
        // TODO: instantiate an instance of your API class
        //
        APIMASH_TomTom.TomTomApi _tomTomApi = new APIMASH_TomTom.TomTomApi();
        public LeftPanel()
        {
            this.InitializeComponent();

            // intialize generic elements of the view model
            this.DefaultViewModel["AppName"] = App.DisplayName;
            this.DefaultViewModel["NoResults"] = false;
            this.DefaultViewModel["ApiStatus"] = ApiResponseStatus.Default;

            // event callback implementation for dismissing the error panel
            ErrorPanel.Dismissed += (s, e) => this.DefaultViewModel["ApiStatus"] = ApiResponseStatus.Default;

            //
            // TODO: change the reference to reflect your API's view model class, which should include an
            //       ObservableCollecation of results that will get bound to the ListView in this panel.
            //       Add the CollectionChanged event event handler to the ObservableCollection in that view model.
            //            

            this.DefaultViewModel["ApiViewModel"] = _tomTomApi.TomTomViewModel;
            _tomTomApi.TomTomViewModel.Results.CollectionChanged += Results_CollectionChanged;
        }
     
        /// <summary>
        /// Carries out application-specific handling of the item selected in the listview. The synchronization with
        /// the map display is already accomodated.
        /// </summary>
        /// <param name="item">Newly selected item that should be cast to a view model class for further processing</param>
        private async Task ProcessSelectedItem(object item)
        {
            //
            // TODO: replace with code that should execute whenever an item is selected from the ListView
            //
            await _tomTomApi.GetCameraImage(item as APIMASH_TomTom.TomTomCameraViewModel);
        }

        /// <summary>
        /// Refreshes the list of items obtained from the API and populates the view model
        /// </summary>
        /// <param name="box">Bounding box of current map view</param>
        public async Task Refresh(BoundingBox box) 
        {
            //
            // TODO: refresh the items in the panel to reflect points of interest in the current map view. You
            //       will invoke your target API that populates the view model's ObservableCollection and returns
            //       a status object.  The "NoResults" entry in the view model is used to drive the visibility
            //       of text that appears when the query returns no elements (versus providing no feedback).
            //
            //
            this.DefaultViewModel["ApiStatus"] =
                await _tomTomApi.GetCameras(box, this.MaxResults);
            this.DefaultViewModel["NoResults"] = _tomTomApi.TomTomViewModel.Results.Count == 0;
        }   

        #region event handlers (API agnostic)
        // handle synchronization for new selection in the list with the map
        private async void MappableListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // get newly selected and deseleced item
            object newItem = e.AddedItems.FirstOrDefault();
            object oldItem = e.RemovedItems.FirstOrDefault();

            // highlight/unhighlight map pins 
            if (Map != null)
            {
                Map.HighlightPointOfInterestPin(newItem as IMappable, true);
                Map.HighlightPointOfInterestPin(oldItem as IMappable, false);
            }

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
        void Results_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // nothing to do here if there no map to sync with
            if (Map == null) return;

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

                        Map.AddPointOfInterestPin(poiPin, mapItem.Position);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Map.ClearPointOfInterestPins();
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
            if (Map != null)
            {
                Refresh(new BoundingBox(Map.TargetBounds.North, Map.TargetBounds.South,
                    Map.TargetBounds.West, Map.TargetBounds.East));
            }
        }
        #endregion
    }
}
