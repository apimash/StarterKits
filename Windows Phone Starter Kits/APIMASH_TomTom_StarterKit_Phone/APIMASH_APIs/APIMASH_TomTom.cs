using APIMASH;
using APIMASH.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;


//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

//
// TODO: use this example as a template for implementing your location-aware API that will associate points-of-interest
//       with the map functionality built into this application. Th ViewModel class includes the information of interest 
//       you want to show in the left panel of the app, it should implement IMappable so it will have the necessary fields 
//       to associate with push pins on the map
//

namespace APIMASH_TomTom
{
    /// <summary>
    /// View model class for list of cameras returned from a TomTom Traffic Cams query
    /// </summary>
    public class TomTomCameraViewModel : BindableBase, IMappable
    {
        public Int32 Sequence { get; set; }
        public Int32 CameraId { get; set; }
        public String Name { get; set; }
        public String Orientation { get; set; }
        public Int32 RefreshRate { get; set; }

        public Byte[] ImageBytes { get; set; }
       
        private BitmapImage _image;
        public BitmapImage Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        private DateTime _lastRefresh;
        public DateTime LastRefresh
        {
            get { return _lastRefresh; }
            set { SetProperty(ref _lastRefresh, value); }
        }

        public Double DistanceFromCenter { get; set; }

        // IMappable properties
        public LatLong Position { get; set; }
        public string Id { get { return CameraId.ToString(); } }
        public string Label { get { return Sequence.ToString(); } }
    }

    /// <summary>
    /// Class for deserializing raw response data from a TomTom Traffic Cams query
    /// </summary>
    public class TomTomCamerasModel
    {
        #region model data corresponding to raw XML data
        public class cameras
        {
            [XmlElement("camera")]
            public camera[] CameraList { get; set; }
        }

        public class camera
        {
            public Int32 cameraId { get; set; }
            public String cameraName { get; set; }
            public String orientation { get; set; }
            public Boolean tempDisabled { get; set; }
            public Int32 refreshRate { get; set; }
            public String cityCode { get; set; }
            public String provider { get; set; }
            public Double latitude { get; set; }
            public Double longitude { get; set; }
            public String zipCode { get; set; }
        }
        #endregion

        /// <summary>
        /// Copy the desired portions of the deserialized model data to the view model collection of cameras
        /// </summary>
        /// <param name="model">Deserializeed result from API call</param>
        /// <param name="viewModel">Collection of view model items</param>
        /// <param name="centerLatitude">Latitude of center point of current map view</param>
        /// <param name="centerLongitude">Longitude of center point of current map view</param>
        /// <param name="maxResults">Maximum number of results to assign to view model (0 = assign all results)</param>
        /// <returns>Indicator of whether items were left out of the view model due to max size restrictions</returns>
        public static Boolean PopulateViewModel(cameras model, ObservableCollection<TomTomCameraViewModel> viewModel, LatLong centerPoint, Int32 maxResults = 0)
        {
            Int32 sequence = 0;

            // set up a staging list for applying any filters/max # of items returned, etc.
            var stagingList = new List<TomTomCameraViewModel>();

            // clear the view model first
            viewModel.Clear();

            // pull desired fields from model and insert into view model
            if (model.CameraList != null)
                foreach (var camera in
                            (from c in model.CameraList
                             select new TomTomCameraViewModel()
                                 {
                                     CameraId = c.cameraId,
                                     Name = c.cameraName,
                                     Orientation = c.orientation.Replace("Traffic closest to camera is t", "T"),
                                     RefreshRate = c.refreshRate / 1000,
                                     Position = new LatLong(c.latitude, c.longitude),
                                     DistanceFromCenter = MapUtilities.HaversineDistance(centerPoint, new LatLong(c.latitude, c.longitude))
                                 }))
                    stagingList.Add(camera);

            // apply max count if provided
            var resultsWereTruncated = (maxResults > 0) && (stagingList.Count > maxResults);
            foreach (var s in stagingList
                              .OrderBy((c) => c.DistanceFromCenter)
                              .Take(resultsWereTruncated ? maxResults : stagingList.Count))
            {
                s.Sequence = ++sequence;
                viewModel.Add(s);
            }

            return resultsWereTruncated;
        }

        public static void PopulateViewModel(BitmapImage camImage, Byte[] imageBytes, TomTomCameraViewModel viewModel)
        {
            viewModel.Image = camImage;
            viewModel.ImageBytes = imageBytes;
            viewModel.LastRefresh = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// View model class for TomTom API
    /// </summary>
    public sealed class TomTomViewModel : BindableBase
    {
        /// <summary>
        /// Indicates whether camera list was truncated at a max size and other cameras are in the same field of view
        /// </summary>
        public Boolean ResultsTruncated
        {
            get { return _cameraListTruncated; }
            internal set { SetProperty(ref _cameraListTruncated, value); }
        }
        private Boolean _cameraListTruncated;

        /// <summary>
        /// List of cameras returned by a search (bindable to the UI)
        /// </summary>
        public ObservableCollection<TomTomCameraViewModel> Results
        {
            get { return _cameras; }
            internal set { SetProperty(ref _cameras, value); }
        }
        private ObservableCollection<TomTomCameraViewModel> _cameras =
            new ObservableCollection<TomTomCameraViewModel>();
    }

    /// <summary>
    /// Wrapper class for TomTom API
    /// </summary>
    public sealed class TomTomApi : ApiBase
    {
        public TomTomViewModel TomTomViewModel = new TomTomViewModel();
        protected override String _apiKey
        {
            get
            {
                return (Application.Current.Resources.Contains("TomTomAPIKey")) ?
                    Application.Current.Resources["TomTomAPIKey"] as String :
                    String.Empty;
            }
        }

        /// <summary>
        /// Performs a query for traffic cameras within the given BoundingBox, <paramref name="b"/>
        /// </summary>
        /// <param name="b">Bounding box defining area for which to return traffic cams</param>
        /// <param name="maxResults">Maximum number of results to assign to view model (0 = assign all results)</param>
        /// <returns>Status of API call <seealso cref="APIMASH.ApiResponseStatus"/></returns>        
        public async Task<APIMASH.ApiResponseStatus> GetCameras(BoundingBox b, Int32 maxResults = 0)
        {
            // clear the results
            TomTomViewModel.Results.Clear();
            TomTomViewModel.ResultsTruncated = false;            
            
            // invoke the API
            var apiResponse = await Invoke<TomTomCamerasModel.cameras>(
                "http://api.tomtom.com/trafficcams/boxquery?top={0}&bottom={1}&left={2}&right={3}&format=xml&key={4}",
                b.North, b.South, b.West, b.East,
                this._apiKey);

            // if successful, copy relevant portions from model to the view model
            if (apiResponse.IsSuccessStatusCode)
            {
                TomTomViewModel.ResultsTruncated = TomTomCamerasModel.PopulateViewModel(
                    apiResponse.DeserializedResponse,
                    TomTomViewModel.Results,
                    new LatLong((b.North + b.South) / 2, (b.West + b.East) / 2),
                    maxResults);
            }
            else
            {
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        apiResponse.SetCustomStatus("Supplied API key is not valid for this request.", apiResponse.StatusCode);
                        break;

                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.ServiceUnavailable:
                        apiResponse.SetCustomStatus("Problem appears to be at TomTom's site. Please retry later.", apiResponse.StatusCode);
                        break;
                }
            }

            // return the status information
            return apiResponse as APIMASH.ApiResponseStatus;
        }

        /// <summary>
        /// Get latest image for a given camera
        /// </summary>
        /// <param name="camera">Camera object</param>
        /// <returns>Status of API call <seealso cref="APIMASH.ApiResponseStatus"/>. This method will alway return success.</returns>
        public async Task<APIMASH.ApiResponseStatus> GetCameraImage(TomTomCameraViewModel camera)
        {
            BitmapImage cameraImage = null;

            // invoke the API (explicit deserializer provided because the image responses from TomTom don't include a Content-Type header
            var apiResponse = await Invoke<BitmapImage>(
                Deserializers<BitmapImage>.DeserializeImage,
                "https://api.tomtom.com/trafficcams/getfullcam/{0}.jpg?key={1}",
                camera.CameraId,
                this._apiKey);

            // if successful, grab image as deserialized response
            if (apiResponse.IsSuccessStatusCode)
            {
                cameraImage = apiResponse.DeserializedResponse;
            }

            // otherwise, use some stock image to reflect error condition
            else if (apiResponse.StatusCode == HttpStatusCode.NotFound)
            {
                cameraImage = new BitmapImage(new Uri("/APIMASH_APIs;component/Assets/camera404.png", UriKind.Relative));
            }
            else
            {
                cameraImage = new BitmapImage(new Uri("/APIMASH_APIs;component/Assets/cameraError.png", UriKind.Relative));
            }

            // populate the ViewModel with the image
            TomTomCamerasModel.PopulateViewModel(cameraImage, apiResponse.RawResponse, camera);

            // return a success status (there will always be an image returned)
            return ApiResponseStatus.Default;
        }
    }
}