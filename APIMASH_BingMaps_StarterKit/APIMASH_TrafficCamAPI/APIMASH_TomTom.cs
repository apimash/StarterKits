using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_TomTom
{
    /// <summary>
    /// View model class for list of cameras returned from a TomTom Traffic Cams query
    /// </summary>
    public class TomTomCameraViewModel
    {
        public Int32 Sequence { get; set; }
        public Int32 CameraId { get; set; }
        public String Name { get; set; }
        public String Orientation { get; set; }
        public Int32 RefreshRate { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
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
        /// <param name="maxResults">Maximum number of results to assign to view model (0 = assign all results)</param>
        /// <returns>Indicator of whether items were left out of the view model due to max size restrictions</returns>
        public static Boolean PopulateViewModel(cameras model, ObservableCollection<TomTomCameraViewModel> viewModel, Int32 maxResults = 0)
        {
            Int32 sequence = 0;

            // set up a staging list for applying any filters/max # of items returned, etc.
            var stagingList = new List<TomTomCameraViewModel>();

            // clear the view model first
            viewModel.Clear();

            // pull desired fields from model and insert into view model
            if (model.CameraList != null)
                foreach (var camera in
                            (from c in model.CameraList select new TomTomCameraViewModel()
                                {
                                    Sequence = ++sequence,
                                    CameraId = c.cameraId,
                                    Name = c.cameraName,
                                    Orientation = c.orientation.Replace("Traffic closest to camera is t", "T"),
                                    RefreshRate = c.refreshRate,
                                    Latitude = c.latitude,
                                    Longitude = c.longitude
                                }))
                    stagingList.Add(camera);

            // apply max count if provided
            var maxResultsExceeded = (maxResults > 0) && (stagingList.Count > maxResults);
            foreach (var s in stagingList.Take(maxResultsExceeded ? maxResults : stagingList.Count))
                viewModel.Add(s);

            return maxResultsExceeded;
        }
    }

    /// <summary>
    /// Bounding box of latitude/longitude used as input to camera search
    /// </summary>
    public class BoundingBox
    {
        /// <summary>
        /// Northern latitudinal boundary of search
        /// </summary>
        public Double Top { get; set; }

        /// <summary>
        /// Southern latitudinal boundary of search
        /// </summary>
        public Double Bottom { get; set; }

        /// <summary>
        /// Western longitudinal boundary of search
        /// </summary>
        public Double Left { get; set; }

        /// <summary>
        /// Eastern longitudinal boundary of search
        /// </summary>
        public Double Right { get; set; }

        public BoundingBox(Double t, Double b, Double l, Double r)
        {
            Top = t;
            Bottom = b;
            Left = l;
            Right = r;
        }
    }

    /// <summary>
    /// Wrapper class for TomTom API
    /// </summary>
    public sealed class TomTomApi : APIMASH.ApiBase
    {
        public TomTomApi()
        {
            _apiKey = Application.Current.Resources["TomTomAPIKey"] as String;
        }

        /// <summary>
        /// Indicates whether camera list was truncated at a max size and other camera are in the same field of view
        /// </summary>
        public Boolean CameraListTruncated
        {
            get { return _cameraListTruncated; }
            set { SetProperty(ref _cameraListTruncated, value); }
        }
        private Boolean _cameraListTruncated;

         /// <summary>
        /// List of cameras returned by a search (bindable to the UI)
        /// </summary>
        public ObservableCollection<TomTomCameraViewModel> Cameras
        {
            get { return _cameras; }
        }
        private ObservableCollection<TomTomCameraViewModel> _cameras =
            new ObservableCollection<TomTomCameraViewModel>();

        /// <summary>
        /// Performs a query for traffic cameras within the given BoundingBox, <paramref name="b"/>
        /// </summary>
        /// <param name="b">Bounding box defining area for which to return traffic cams</param>
        /// <param name="maxResults">Maximum number of results to assign to view model (0 = assign all results)</param>
        /// <returns>Status of API call <seealso cref="APIMASH.ApiResponseStatus"/></returns>        
        public async Task<APIMASH.ApiResponseStatus> GetCameras(BoundingBox b, Int32 maxResults = 0)
        {

            // invoke the API
            var apiResponse = await Invoke<TomTomCamerasModel.cameras>(
                "http://api.tomtom.com/trafficcams/boxquery?top={0}&bottom={1}&left={2}&right={3}&format=xml&key={4}",
                b.Top, b.Bottom, b.Left, b.Right,
                this._apiKey);

            // clear the results
            Cameras.Clear();
            CameraListTruncated = false;

            // if successful, copy relevant portions from model to the view model
            if (apiResponse.IsSuccessStatusCode)
            {
                CameraListTruncated = TomTomCamerasModel.PopulateViewModel(apiResponse.DeserializedResponse, _cameras, maxResults);
            }
            else
            {
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.Forbidden:
                        apiResponse.Message = "Supplied API key is not valid for this request.";
                        break;
                    case HttpStatusCode.InternalServerError:
                        apiResponse.Message = "Problem appears to be at TomTom's site. Please retry later.";
                        break;
                }   
            }

            // return the status information
            return apiResponse as APIMASH.ApiResponseStatus;
        }

        /// <summary>
        /// Gets camera image 
        /// </summary>
        /// <param name="cameraId">Camera identfier as determined by previous call to <see cref="GetCameras"/></param>
        /// <returns>Bitmap of the current camera image</returns>
        public BitmapImage GetCameraImage(Int32 cameraId)
        {
           return new BitmapImage(
                    new Uri(
                        String.Format("https://api.tomtom.com/trafficcams/getfullcam/{0}.jpg?key={1}", 
                        cameraId,
                        this._apiKey)
                    ));
        }
    }
}