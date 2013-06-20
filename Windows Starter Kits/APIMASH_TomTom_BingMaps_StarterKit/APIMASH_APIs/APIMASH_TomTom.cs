using APIMASH;
using APIMASH.Mapping;
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

        private Byte[] _imageBytes;
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
                                     RefreshRate = c.refreshRate,
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
                return (Application.Current.Resources.ContainsKey("TomTomAPIKey")) ?
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
                cameraImage = new BitmapImage(new Uri("ms-appx:///APIMASH_APIs/Assets/camera404.png"));
            }
            else
            {
                cameraImage = new BitmapImage(new Uri("ms-appx:///APIMASH_APIs/Assets/cameraError.png"));
            }

            // populate the ViewModel with the image
            TomTomCamerasModel.PopulateViewModel(cameraImage, apiResponse.RawResponse, camera);

            // return a success status (there will always be an image returned)
            return ApiResponseStatus.Default;
        }

        /// <summary>
        /// Pre-populated list of search results based on known positions of traffic cameras
        /// </summary>
        public static IEnumerable<IMappable> SearchSuggestionList = new List<IMappable>()
        {
            //
            // TODO: (option) if implementing the Search contract, you can provide search result suggestions by
            //       populating this static list of IMappable objects. Up to five of these suggestions will show
            //       automatically in the Search charm pane as the user edits the query text. Selecting one of the 
            //       known search results will navigate them to a location with known points-of-interest in the vicinity.
            //
            new SearchResultSuggestion("Albany, New York", 42.6519355773926, -73.7580604553223),
            new SearchResultSuggestion("Allentown, Pennsylvania", 40.6058902740479, -75.4747734069824),
            new SearchResultSuggestion("Altoona, Pennsylvania", 40.5032196044922, -78.3982429504395),
            new SearchResultSuggestion("Aspen, Colorado", 39.1916809082031, -106.818893432617),
            new SearchResultSuggestion("Atlanta, Georgia", 33.7658843994141, -84.3905258178711),
            new SearchResultSuggestion("Atlantic City, New Jersey", 39.360897064209, -74.4287147521973),
            new SearchResultSuggestion("Austin, Texas", 30.270583152771, -97.7432174682617),
            new SearchResultSuggestion("Bakersfield, California", 35.3678379058838, -119.007053375244),
            new SearchResultSuggestion("Baltimore, Maryland", 39.2896995544434, -76.6152229309082),
            new SearchResultSuggestion("Baton Rouge, Louisiana", 30.4524784088135, -91.167366027832),
            new SearchResultSuggestion("Beaumont, Texas", 30.0852003097534, -94.1111450195313),
            new SearchResultSuggestion("Bellingham, Washington", 48.7539825439453, -122.475440979004),
            new SearchResultSuggestion("Blacksburg, Virginia", 37.2259368896484, -80.4153442382813),
            new SearchResultSuggestion("Blaine, Washington", 48.9895496368408, -122.74328994751),
            new SearchResultSuggestion("Boise, Idaho", 43.6128158569336, -116.201805114746),
            new SearchResultSuggestion("Boston, Massachusetts", 42.3488216400146, -71.0644416809082),
            new SearchResultSuggestion("Boulder, Colorado", 40.0206031799316, -105.276187896729),
            new SearchResultSuggestion("Bradford, Pennsylvania", 41.9597434997559, -78.6381683349609),
            new SearchResultSuggestion("Bragdon, Colorado", 38.4018707275391, -104.616287231445),
            new SearchResultSuggestion("Bridgeport, Connecticut", 41.1859130859375, -73.192497253418),
            new SearchResultSuggestion("Bristol, Virginia", 36.5984649658203, -82.1809349060059),
            new SearchResultSuggestion("Buffalo, New York", 42.8831424713135, -78.8704452514648),
            new SearchResultSuggestion("Cambridge, Maryland", 38.5627346038818, -76.0757293701172),
            new SearchResultSuggestion("Canaan, New York", 42.412748336792, -73.4510116577148),
            new SearchResultSuggestion("Canton, North Carolina", 35.5369529724121, -82.8386306762695),
            new SearchResultSuggestion("Charleston, South Carolina", 32.7942314147949, -79.9403305053711),
            new SearchResultSuggestion("Charlotte, North Carolina", 35.2227897644043, -80.8390655517578),
            new SearchResultSuggestion("Charlottesville, Virginia", 38.0293006896973, -78.4766998291016),
            new SearchResultSuggestion("Chattanooga, Tennessee", 35.041955947876, -85.3094711303711),
            new SearchResultSuggestion("Chicago, Illinois", 41.9007797241211, -87.635871887207),
            new SearchResultSuggestion("Cincinnati, Ohio", 39.1041355133057, -84.5099487304688),
            new SearchResultSuggestion("Cleveland, Ohio", 41.5029144287109, -81.6821517944336),
            new SearchResultSuggestion("Colby , Kansas", 39.3957862854004, -101.046684265137),
            new SearchResultSuggestion("Colorado Springs, Colorado", 38.8373966217041, -104.816703796387),
            new SearchResultSuggestion("Columbia, South Carolina", 33.9997978210449, -81.0406684875488),
            new SearchResultSuggestion("Columbus, Ohio", 39.9567031860352, -83.0016021728516),
            new SearchResultSuggestion("Concord, New Hampshire", 43.2050762176514, -71.5388488769531),
            new SearchResultSuggestion("Cumberland, Maryland", 39.6458377838135, -78.7582092285156),
            new SearchResultSuggestion("Dallas, Texas", 32.7770690917969, -96.7961730957031),
            new SearchResultSuggestion("Dallesport, Washington", 45.6162300109863, -121.145317077637),
            new SearchResultSuggestion("Danville, California", 37.8268585205078, -122.004035949707),
            new SearchResultSuggestion("Danville, Virginia", 36.5910968780518, -79.4109420776367),
            new SearchResultSuggestion("Dayton, Ohio", 39.7497138977051, -84.1901397705078),
            new SearchResultSuggestion("Daytona Beach, Florida", 29.207221031189, -81.0344085693359),
            new SearchResultSuggestion("Denver, Colorado", 39.7411041259766, -104.984230041504),
            new SearchResultSuggestion("Des Moines, Iowa", 41.5926189422607, -93.614860534668),
            new SearchResultSuggestion("Detroit, Michigan", 42.3321533203125, -83.0467300415039),
            new SearchResultSuggestion("Dover, Delaware", 39.1552925109863, -75.5247688293457),
            new SearchResultSuggestion("Dubois, Pennsylvania", 41.1204414367676, -78.7600708007813),
            new SearchResultSuggestion("Duluth, Minnesota", 46.7891407012939, -92.0956344604492),
            new SearchResultSuggestion("Durango, Colorado", 37.2753982543945, -107.879016876221),
            new SearchResultSuggestion("Durham, North Carolina", 35.9986248016357, -78.9024047851563),
            new SearchResultSuggestion("Easton, Maryland", 38.7737007141113, -76.0707397460938),
            new SearchResultSuggestion("Emporia, Kansas", 38.4097557067871, -96.1848907470703),
            new SearchResultSuggestion("Erie, Pennsylvania", 42.11741065979, -80.0929222106934),
            new SearchResultSuggestion("Eureka, California", 40.7978324890137, -124.153026580811),
            new SearchResultSuggestion("Florence, South Carolina", 34.191951751709, -79.772159576416),
            new SearchResultSuggestion("Fort Collins, Colorado", 40.5739631652832, -105.075054168701),
            new SearchResultSuggestion("Fort Lauderdale, Florida", 26.119083404541, -80.1435432434082),
            new SearchResultSuggestion("Franconia, New Hampshire", 44.2270202636719, -71.7462921142578),
            new SearchResultSuggestion("Fresno, California", 36.7493705749512, -119.782852172852),
            new SearchResultSuggestion("Frisco, Colorado", 39.5681858062744, -106.116481781006),
            new SearchResultSuggestion("Ft. Worth, Texas", 32.7514305114746, -97.3295211791992),
            new SearchResultSuggestion("Garden City, Kansas", 37.9710102081299, -100.871196746826),
            new SearchResultSuggestion("Geneva, New York", 42.8651218414307, -76.9921188354492),
            new SearchResultSuggestion("George, Washington", 47.0770721435547, -119.864917755127),
            new SearchResultSuggestion("Georgetown, Delaware", 38.6919860839844, -75.3852195739746),
            new SearchResultSuggestion("Goodland, Kansas", 39.3553485870361, -101.713859558105),
            new SearchResultSuggestion("Grand Junction, Colorado", 39.0698261260986, -108.56360244751),
            new SearchResultSuggestion("Grand Rapids, Michigan", 42.9581832885742, -85.6643104553223),
            new SearchResultSuggestion("Grantham, New Hampshire", 43.4903697967529, -72.1373405456543),
            new SearchResultSuggestion("Greensboro, North Carolina", 36.0716953277588, -79.8031234741211),
            new SearchResultSuggestion("Halifax, Nova Scotia", 44.6557941436768, -63.5884094238281),
            new SearchResultSuggestion("Hamilton, Ontario", 43.2600135803223, -79.876335144043),
            new SearchResultSuggestion("Harrisburg, Pennsylvania", 40.2636222839355, -76.8805770874023),
            new SearchResultSuggestion("Harrisonburg, Virginia", 38.4496002197266, -78.868896484375),
            new SearchResultSuggestion("Hartford, Connecticut", 41.752758026123, -72.6955184936523),
            new SearchResultSuggestion("Hays, Kansas", 38.8756141662598, -99.3218688964844),
            new SearchResultSuggestion("Houma, Louisiana", 29.5930471420288, -90.7121391296387),
            new SearchResultSuggestion("Houston, Texas", 29.7529296875, -95.348762512207),
            new SearchResultSuggestion("Houston, Texas", 29.7529296875, -95.348762512207),
            new SearchResultSuggestion("Huntsville, Texas", 30.7204256057739, -95.5509910583496),
            new SearchResultSuggestion("Indianapolis IN", 39.768892288208, -86.1523666381836),
            new SearchResultSuggestion("Jacksonville, Florida", 30.3308477401733, -81.6537590026855),
            new SearchResultSuggestion("Jamestown, New York", 42.0958862304688, -79.2380561828613),
            new SearchResultSuggestion("Johnstown, Pennsylvania", 40.3233871459961, -78.919303894043),
            new SearchResultSuggestion("Kansas City, Missouri", 39.1021156311035, -94.5762023925781),
            new SearchResultSuggestion("Keene, New Hampshire", 42.9348297119141, -72.2810707092285),
            new SearchResultSuggestion("Kenosha, Wisconsin", 42.5834827423096, -87.8214797973633),
            new SearchResultSuggestion("Key West, Florida", 24.5552062988281, -81.7795906066895),
            new SearchResultSuggestion("Kitty Hawk, North Carolina", 36.0799999237061, -75.7079772949219),
            new SearchResultSuggestion("Knoxville, Tennessee", 35.9629383087158, -83.9192657470703),
            new SearchResultSuggestion("Laconia, New Hampshire", 43.5279293060303, -71.4721908569336),
            new SearchResultSuggestion("Lafayette, Louisiana", 30.2163133621216, -92.0202331542969),
            new SearchResultSuggestion("Lake Charles, Louisiana", 30.2269906997681, -93.2196846008301),
            new SearchResultSuggestion("Lakeland, Florida", 28.0436601638794, -81.941951751709),
            new SearchResultSuggestion("Lancaster, Texas", 32.6008205413818, -96.7605476379395),
            new SearchResultSuggestion("Las Vegas NV", 36.1677742004395, -115.15710067749),
            new SearchResultSuggestion("Lawrence, Kansas", 38.9679298400879, -95.239631652832),
            new SearchResultSuggestion("Lawrence, Massachusetts", 42.7098236083984, -71.1599426269531),
            new SearchResultSuggestion("Linden, New Jersey", 40.6257095336914, -74.2475929260254),
            new SearchResultSuggestion("Lock Haven, Pennsylvania", 41.1321487426758, -77.4440879821777),
            new SearchResultSuggestion("London, Ontario", 42.9868183135986, -81.2376136779785),
            new SearchResultSuggestion("Los Angeles, California", 34.0546951293945, -118.241455078125),
            new SearchResultSuggestion("Louisville, Kentucky", 38.2482204437256, -85.7699661254883),
            new SearchResultSuggestion("Loveland, Colorado", 40.3974380493164, -105.077686309814),
            new SearchResultSuggestion("Lufkin, Texas", 31.3375253677368, -94.7215118408203),
            new SearchResultSuggestion("Lynchburg, Virginia", 37.4054279327393, -79.1504096984863),
            new SearchResultSuggestion("Macon, Georgia", 32.8391227722168, -83.6361846923828),
            new SearchResultSuggestion("Madera, California", 36.9632759094238, -120.059219360352),
            new SearchResultSuggestion("Madison, Wisconsin", 43.0742721557617, -89.3847198486328),
            new SearchResultSuggestion("Manchester, New Hampshire", 42.9902095794678, -71.4628257751465),
            new SearchResultSuggestion("Manhattan, Kansas", 39.1864166259766, -96.5882301330566),
            new SearchResultSuggestion("Mansfield, Ohio", 40.7600212097168, -82.5197677612305),
            new SearchResultSuggestion("Marquette, Michigan", 46.5448780059814, -87.4026489257813),
            new SearchResultSuggestion("Martinsville, Virginia", 36.6873378753662, -79.8729438781738),
            new SearchResultSuggestion("Melbourne, Florida", 28.0796184539795, -80.6069374084473),
            new SearchResultSuggestion("Memphis, Tennessee", 35.1460056304932, -90.0468711853027),
            new SearchResultSuggestion("Miami, Florida", 25.7982177734375, -80.2096328735352),
            new SearchResultSuggestion("Millville, New Jersey", 39.3957347869873, -75.0352897644043),
            new SearchResultSuggestion("Milwaukee, Wisconsin", 43.0414276123047, -87.907657623291),
            new SearchResultSuggestion("Minneapolis, Minnesota", 44.9758911132813, -93.273567199707),
            new SearchResultSuggestion("Monroe, Louisiana", 32.5088806152344, -92.113712310791),
            new SearchResultSuggestion("Morristown, New Jersey", 40.7955665588379, -74.4786720275879),
            new SearchResultSuggestion("Moses Lake, Washington", 47.1272087097168, -119.27730178833),
            new SearchResultSuggestion("Myrtle Beach, South Carolina", 33.6937351226807, -78.8845558166504),
            new SearchResultSuggestion("Nashville, Tennessee", 36.1635036468506, -86.7816200256348),
            new SearchResultSuggestion("New Haven, Connecticut", 41.3025169372559, -72.928279876709),
            new SearchResultSuggestion("New London, Connecticut", 41.3560485839844, -72.0980682373047),
            new SearchResultSuggestion("New Orleans, Louisiana", 29.972146987915, -90.0646209716797),
            new SearchResultSuggestion("New Paltz, New York", 41.7491359710693, -74.0890731811523),
            new SearchResultSuggestion("New York, New York", 40.710147857666, -74.0065727233887),
            new SearchResultSuggestion("Newark, New Jersey", 40.7379093170166, -74.1745910644531),
            new SearchResultSuggestion("Newburgh, New York", 41.5076885223389, -74.0158233642578),
            new SearchResultSuggestion("Newport News, Virginia", 37.0306186676025, -76.4524841308594),
            new SearchResultSuggestion("Niagara Falls, New York", 43.0834007263184, -79.0706024169922),
            new SearchResultSuggestion("Norfolk, Virginia", 36.848970413208, -76.2816848754883),
            new SearchResultSuggestion("Ocean City, Maryland", 38.3330497741699, -75.0857696533203),
            new SearchResultSuggestion("Ogden, Utah", 41.2266368865967, -111.963726043701),
            new SearchResultSuggestion("Olympia, Washington", 47.0411834716797, -122.889419555664),
            new SearchResultSuggestion("Ontario, California", 34.0678882598877, -117.645401000977),
            new SearchResultSuggestion("Ontario, Oregon", 44.0268020629883, -116.963329315186),
            new SearchResultSuggestion("Orlando, Florida", 28.5634822845459, -81.3723907470703),
            new SearchResultSuggestion("Ottawa, Ontario", 45.4255905151367, -75.7010726928711),
            new SearchResultSuggestion("Palm Beach, Florida", 26.710223197937, -80.0375213623047),
            new SearchResultSuggestion("Pasco, Washington", 46.2326622009277, -119.090919494629),
            new SearchResultSuggestion("Philadelphia, Pennsylvania", 39.9490661621094, -75.1605911254883),
            new SearchResultSuggestion("Phoenix, Arizona", 33.4504127502441, -112.073535919189),
            new SearchResultSuggestion("Pittsburg, Pennsylvania", 40.9342498779297, -80.3635025024414),
            new SearchResultSuggestion("Portland, Oregon", 45.5195770263672, -122.685134887695),
            new SearchResultSuggestion("Portsmouth, New Hampshire", 43.0727214813232, -70.7664031982422),
            new SearchResultSuggestion("Provo, Utah", 40.2449283599854, -111.663730621338),
            new SearchResultSuggestion("Raleigh, North Carolina", 35.7856388092041, -78.6439895629883),
            new SearchResultSuggestion("Redding, California", 40.5888156890869, -122.387828826904),
            new SearchResultSuggestion("Richland, Texas", 31.9265851974487, -96.4277610778809),
            new SearchResultSuggestion("Richmond, Virginia", 37.5424022674561, -77.4380531311035),
            new SearchResultSuggestion("Roanoke, Virginia", 37.2708549499512, -79.9467430114746),
            new SearchResultSuggestion("Rochester, Minnesota", 44.0226860046387, -92.4598388671875),
            new SearchResultSuggestion("Rochester, New York", 43.158576965332, -77.618293762207),
            new SearchResultSuggestion("Rockwood, Tennessee", 35.8652477264404, -84.6874046325684),
            new SearchResultSuggestion("Ronkonkoma, New York", 40.803638458252, -73.1237411499023),
            new SearchResultSuggestion("Sacramento, California", 38.5763854980469, -121.493591308594),
            new SearchResultSuggestion("Sagamore, Massachusetts", 41.7706184387207, -70.5302505493164),
            new SearchResultSuggestion("Salina, Kansas", 38.8446788787842, -97.6102485656738),
            new SearchResultSuggestion("Salisbury, Maryland", 38.3625926971436, -75.594165802002),
            new SearchResultSuggestion("Salt Lake City, Utah", 40.7601051330566, -111.890781402588),
            new SearchResultSuggestion("Sandusky, Ohio", 41.4465789794922, -82.7059860229492),
            new SearchResultSuggestion("Santa Ana, California", 33.7440719604492, -117.872779846191),
            new SearchResultSuggestion("Sarnia, Ontario", 42.9814586639404, -82.3987083435059),
            new SearchResultSuggestion("Sault St. Marie, Michigan", 46.4880218505859, -84.3506050109863),
            new SearchResultSuggestion("Schenectady, New York", 42.8148574829102, -73.9287719726563),
            new SearchResultSuggestion("Scranton, Pennsylvania", 41.4282779693604, -75.657958984375),
            new SearchResultSuggestion("Seattle, Washington", 47.5997772216797, -122.334579467773),
            new SearchResultSuggestion("Shreveport, Louisiana", 32.4952564239502, -93.7586822509766),
            new SearchResultSuggestion("Spartanburg, South Carolina", 34.9507827758789, -81.9304008483887),
            new SearchResultSuggestion("Spokane, Washington", 47.6605968475342, -117.406558990479),
            new SearchResultSuggestion("St. George, Utah", 37.107177734375, -113.576164245605),
            new SearchResultSuggestion("St. Louis, Missouri", 38.6268329620361, -90.2123870849609),
            new SearchResultSuggestion("State College, Pennsylvania", 40.7922172546387, -77.859748840332),
            new SearchResultSuggestion("Staunton, Virginia", 38.1513824462891, -79.074951171875),
            new SearchResultSuggestion("Sterling, Colorado", 40.6251659393311, -103.20955657959),
            new SearchResultSuggestion("Syracuse, New York", 43.0545520782471, -76.1490173339844),
            new SearchResultSuggestion("Tacoma, Washington", 47.2612743377686, -122.454471588135),
            new SearchResultSuggestion("Tampa, Florida", 27.954927444458, -82.4593811035156),
            new SearchResultSuggestion("Toledo, Ohio", 41.6592864990234, -83.53466796875),
            new SearchResultSuggestion("Topeka, Kansas", 39.048849105835, -95.6775703430176),
            new SearchResultSuggestion("Toronto, Ontario", 43.6531715393066, -79.382698059082),
            new SearchResultSuggestion("Trenton, New Jersey", 40.2214965820313, -74.762321472168),
            new SearchResultSuggestion("Tucson, Arizona", 32.2238292694092, -110.973079681396),
            new SearchResultSuggestion("Utica, New York", 43.0986785888672, -75.2452621459961),
            new SearchResultSuggestion("Vancouver, Washington", 45.6371116638184, -122.652282714844),
            new SearchResultSuggestion("Visalia, California", 36.331470489502, -119.296298980713),
            new SearchResultSuggestion("Washington DC", 38.8999824523926, -77.0226974487305),
            new SearchResultSuggestion("Wells, Michigan", 45.778056,-87.117222),
            new SearchResultSuggestion("Wichita, Kansas", 37.6855487823486, -97.3271751403809),
            new SearchResultSuggestion("Wiggins, Colorado", 40.2297477722168, -104.074676513672),
            new SearchResultSuggestion("Wilmington, North Carolina", 34.2282257080078, -77.9453353881836),
            new SearchResultSuggestion("Winchester, Virginia", 39.1759872436523, -78.1602478027344),
            new SearchResultSuggestion("Winston Salem, North Carolina", 36.1059322357178, -80.2569885253906),
            new SearchResultSuggestion("Woodbine, New Jersey", 39.2397899627686, -74.8152122497559),
            new SearchResultSuggestion("Wyandotte, Michigan", 42.2010593414307, -83.1530685424805),
            new SearchResultSuggestion("York, Pennsylvania", 39.9608287811279, -76.7201194763184)
        };
    }
}