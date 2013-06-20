using APIMASH;
using APIMASH.Mapping;
using APIMASH_StarterKit.Common;
using APIMASH_StarterKit.Mapping;
using Bing.Maps;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH_StarterKit
{
    /// <summary>
    /// Event arguments providing the previous and currently selected item from the ListView in the panel
    /// </summary>
    public class ItemSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Item currently selected (possibly null)
        /// </summary>
        public IMappable NewItem { get; private set; }

        /// <summary>
        /// Item previously selected (possibly null)
        /// </summary>
        public IMappable OldItem { get; private set; }

        public ItemSelectedEventArgs(object newItem, object oldItem)
        {
            NewItem = newItem as IMappable;            
            OldItem = oldItem as IMappable;
        }
    }

    /// <summary>
    /// Implementation of left-side panel displaying API-specific points of interest, with synchronization to
    /// Bing Maps control built-in.
    /// </summary>
    public sealed partial class LeftPanel : LayoutAwarePanel
    {
        /// <summary>
        /// Reference to map on the main page
        /// </summary>
        public Map Map { get; set; }

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

        #region Refreshed event handler
        /// <summary>
        /// Occurs when one results in the panel are refreshed allowing parent control to take appropriate actions
        /// </summary>
        public event EventHandler<EventArgs> Refreshed;
        private void OnRefreshed()
        {
            if (Refreshed != null) Refreshed(this, new EventArgs());
        }
        #endregion

        #region ItemSelected handler
        /// <summary>
        /// Occurs when item in the ListView of the panel is selected
        /// </summary>
        public event EventHandler<ItemSelectedEventArgs> ItemSelected;
        private void OnItemSelected(ItemSelectedEventArgs e)
        {
            if (ItemSelected != null) ItemSelected(this, e);
        }
        #endregion

        //
        // TODO: create an instance of your API class
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
            // TODO: change the references below to reflect your API's view model class, which should include an
            //       ObservableCollecation of results that will get bound to the ListView in this panel.
            //       Add the CollectionChanged event event handler to the ObservableCollection in that view model
            //       (no changes are required to the inner implementation of Results_CollectionChanged).
            //            

            this.DefaultViewModel["ApiViewModel"] = _tomTomApi.TomTomViewModel;
            _tomTomApi.TomTomViewModel.Results.CollectionChanged += Results_CollectionChanged;
        }

        //
        // TODO: implement code needed to share item from main page
        //
public async void GetSharedData(DataTransferManager sender, DataRequestedEventArgs args)
{
    try
    {
        var currentCam = MappableListView.SelectedItem as APIMASH_TomTom.TomTomCameraViewModel;
        if (currentCam != null)
        {

            DataRequestDeferral deferral = args.Request.GetDeferral();

            args.Request.Data.Properties.Title = String.Format("TomTom Camera: {0}", currentCam.CameraId);
            args.Request.Data.Properties.Description = currentCam.Name;

            // share a file
            var file = await StorageFile.CreateStreamedFileAsync(
                String.Format("{0}_{1}.jpg", currentCam.CameraId, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")),
                async stream =>
                {
                    await stream.WriteAsync(currentCam.ImageBytes.AsBuffer());
                    await stream.FlushAsync();
                    stream.Dispose();
                },
                null);
            args.Request.Data.SetStorageItems(new List<IStorageItem> { file });

            // share as bitmap
            InMemoryRandomAccessStream raStream = new InMemoryRandomAccessStream();
            await raStream.WriteAsync(currentCam.ImageBytes.AsBuffer());
            await raStream.FlushAsync();
            args.Request.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(raStream));

            deferral.Complete();
        }
        else
        {
            args.Request.FailWithDisplayText("Select a camera to share its image.");
        }
    }
    catch (Exception ex)
    {
        args.Request.FailWithDisplayText(ex.Message);
    }
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
        /// <param name="id">Id of IMappable item that should be selected</param>
        public async Task Refresh(BoundingBox box, String id = null) 
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


            // if there's an IMappable ID provided, select that item automatically
            if (id != null)
                MappableListView.SelectedItem = MappableListView.Items.Where((c) => (c as IMappable).Id == id).FirstOrDefault();

            // signal that panel has been refreshed
            OnRefreshed();
        }   

        #region event handlers (API agnostic thus requiring no modification)

        // handle synchronization for new selection in the list with the map
        AsyncLock _lock = new AsyncLock();
        private async void MappableListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (await _lock.LockAsync())
            {

                // get newly selected and deselected item
                object newItem = e.AddedItems.FirstOrDefault();
                object oldItem = e.RemovedItems.FirstOrDefault();

                // process new selection
                if (newItem != null)
                    await ProcessSelectedItem(newItem);

                // HACK ALERT: explicitly setting visibility because a reset of list isn't triggering the rebinding so 
                // that the XAML converter for Visibility kicks in.  
                MappableItem.Visibility = newItem == null ? Visibility.Collapsed : Visibility.Visible;

                // attach handler to ensure selected item is in view after layout adjustments
                MappableListView.LayoutUpdated += MappableListView_LayoutUpdated;

                // notify event listeners that new item has been selected
                OnItemSelected(new ItemSelectedEventArgs(newItem, oldItem));
            }
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
            // the synchronization requires a reference to a Bing.Maps object on the Main page
            if (Map == null)
                throw new System.NullReferenceException("An instance of Bing.Maps is required here, yet the Map property was found to be null."); 
                
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
