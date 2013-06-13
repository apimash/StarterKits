using APIMASH;
using APIMASH.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH_BingMaps
{
    /// <summary>
    /// View model class for list of locations returned from a Bing Maps search
    /// </summary>
    public class BingMapsLocationViewModel : BindableBase, IEquatable<BingMapsLocationViewModel>
    {
        /// <summary>
        /// Address (taken from Address.AddressLine)
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// City (taken from Address.Locality)
        /// </summary>
        public String City { get; set; }

        /// <summary>
        /// State (taken from Address.AdminDistrict)
        /// </summary>
        public String State { get; set; }

        /// <summary>
        /// Position (average of the boundng box, bbox)
        /// </summary>
        public LatLong Position { get; set; }

        /// <summary>
        /// Distance in meters of longest side of the bounding box, bbox)
        /// </summary>
        public Double Extent { get; set; }

        /// <summary>
        /// Static map tile used in search results page
        /// </summary>
        public BitmapImage Tile
        {
            get { return _tile; }
            set { SetProperty(ref _tile, value); }
        }
        private BitmapImage _tile;

        /// <summary>
        /// Determines whether two location results should be considered equal
        /// </summary>
        /// <param name="other">location to which to compare current instance</param>
        /// <returns>true if two locations should be considered identical</returns>
        public bool Equals(BingMapsLocationViewModel other)
        {
            return ((this.Address == other.Address) && (this.City == other.City) && (this.State == other.State));
        }


        public BingMapsLocationViewModel()
        {
            // setup a placeholder image (1x1 pixel) used for filler in visual displays
            Tile = new BitmapImage(new Uri("ms-appx:///APIMASH_APIs/Assets/ghost.png"));
        }
    }

    /// <summary>
    /// Class for deserializing raw response data from a Bing Maps location query
    /// </summary>
    public class BingMapsLocationsModel
    {
        /// <summary>
        /// Root node of reponse data in raw format (JSON)
        /// </summary>
        public RootObject ModelData { get; private set; }

        #region model data generated via http://json2csharp.com
        public class Point
        {
            public string type { get; set; }
            public List<double> coordinates { get; set; }
        }

        public class Address
        {
            public string addressLine { get; set; }
            public string adminDistrict { get; set; }
            public string adminDistrict2 { get; set; }
            public string countryRegion { get; set; }
            public string formattedAddress { get; set; }
            public string locality { get; set; }
            public string landmark { get; set; }
        }

        public class GeocodePoint
        {
            public string type { get; set; }
            public List<double> coordinates { get; set; }
            public string calculationMethod { get; set; }
            public List<string> usageTypes { get; set; }
        }

        [DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1", Name = "Location")]
        public class Resource
        {
            [DataMember]
            public string __type { get; set; }
            [DataMember]
            public List<double> bbox { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public Point point { get; set; }
            [DataMember]
            public Address address { get; set; }
            [DataMember]
            public string confidence { get; set; }
            [DataMember]
            public string entityType { get; set; }
            [DataMember]
            public List<GeocodePoint> geocodePoints { get; set; }
            [DataMember]
            public List<string> matchCodes { get; set; }
        }

        public class ResourceSet
        {
            public int estimatedTotal { get; set; }
            public List<Resource> resources { get; set; }
        }

        public class RootObject
        {
            public string authenticationResultCode { get; set; }
            public string brandLogoUri { get; set; }
            public string copyright { get; set; }
            public List<ResourceSet> resourceSets { get; set; }
            public int statusCode { get; set; }
            public string statusDescription { get; set; }
            public string traceId { get; set; }
        }
        #endregion

        /// <summary>
        /// Selects best candidate for view model "Address" field given the entity type
        /// </summary>
        /// <param name="mapResult">Resource returned from Bing Maps query</param>
        /// <returns>String containing text to be used as the "address" in resulting view model</returns>
        private static String GetFormattedAddress(Resource mapResult)
        {
            if (mapResult.entityType == "Neighborhood")
                return mapResult.address.locality;
            else
                return mapResult.address.landmark ?? mapResult.address.addressLine;
        }

        /// <summary>
        /// Selects best candidate for view model "City" field given the entity type
        /// </summary>
        /// <param name="mapResult">Resource returned from Bing Maps query</param>
        /// <returns>String containing text to be used as the "city" in resulting view model</returns>
        private static String GetFormattedCity(Resource mapResult)
        {
            if (mapResult.entityType == "AdminDivision1")
                return String.Empty;
            else if (mapResult.entityType == "AdminDivision2")
                return mapResult.address.adminDistrict2;
            else if (mapResult.entityType == "Neighborhood")
                return mapResult.address.adminDistrict2;
            else
                return mapResult.address.locality;
        }

        /// <summary>
        /// Selects best candidate for view model "State" field given the entity type
        /// </summary>
        /// <param name="mapResult">Resource returned from Bing Maps query</param>
        /// <returns>String containing text to be used as the "state" in resulting view model</returns>
        private static String GetFormattedState(Resource mapResult)
        {
            if (mapResult.entityType == "AdminDivision1")
                return mapResult.name;
            else
                return mapResult.address.adminDistrict;
        }

        /// <summary>
        /// Copies the desired portions of the deserialized model data to the view model collection of locations
        /// </summary>
        /// <param name="model">Deserialized result from API call</param>
        /// <param name="maxResults">Maximum number of results to assign to view model (0 = assign all results)</param>
        /// <returns>Indicator of whether items were left out of the view model due to max size restrictions</returns>
        public static Boolean PopulateViewModel(RootObject model, ObservableCollection<BingMapsLocationViewModel> viewModel, Int32 maxResults = 0)
        {
            // filter criteria
            String[] countryList = { "United States", "Canada" };

            // set up a staging list for applying any filters/max # of items returned, etc.
            var stagingList = new List<BingMapsLocationViewModel>();

            // clear the view model first
            viewModel.Clear();

            // loop through resource sets (there should only be one)
            foreach (var resourceSet in model.resourceSets)
            {
                // loop through resources in resource set
                foreach (var resource in resourceSet.resources.Where((r) => countryList.Contains(r.address.countryRegion)))
                {

                    // add location to staging list list
                    stagingList.Add(new BingMapsLocationViewModel()
                    {
                        Address = GetFormattedAddress(resource),
                        City = GetFormattedCity(resource),
                        State = GetFormattedState(resource),
                        Position = new LatLong((resource.bbox[0] + resource.bbox[2]) / 2,(resource.bbox[1] + resource.bbox[3]) / 2),
                        Extent = Math.Max(
                             MapUtilities.HaversineDistance(new LatLong(resource.bbox[0], resource.bbox[1]), new LatLong(resource.bbox[2], resource.bbox[1])),
                             MapUtilities.HaversineDistance(new LatLong(resource.bbox[0], resource.bbox[1]), new LatLong(resource.bbox[0], resource.bbox[3]))
                             )
                    });
                }
            }

            // only show results that appear unique
            var uniqueStagingList = stagingList.Distinct();

            // apply max count if provided
            var maxResultsExceeded = (maxResults > 0) && (uniqueStagingList.Count() > maxResults);
            foreach (var s in uniqueStagingList.Take(maxResultsExceeded ? maxResults : uniqueStagingList.Count()))
                viewModel.Add(s);

            return maxResultsExceeded;
        }
    }

    /// <summary>
    /// View model class for all Bing API operations
    /// </summary>
    public class BingMapsViewModel : BindableBase
    {
        /// <summary>
        /// List of locations returned by a search (bindable to the UI)
        /// </summary>
        public ObservableCollection<BingMapsLocationViewModel> Results
        {
            get { return _locations; }
            set { SetProperty(ref _locations, value); }
        }
        private ObservableCollection<BingMapsLocationViewModel> _locations =
            new ObservableCollection<BingMapsLocationViewModel>();
    }

    /// <summary>
    /// Wrapper class for Bing Maps API
    /// </summary>
    public sealed class BingMapsApi : APIMASH.ApiBase
    {
        public BingMapsViewModel BingMapsViewModel = new BingMapsViewModel();

        // BingMaps API calls will use a "session key" option available when used in conjunction
        // with the Bing Maps control. The session key incurs one transaction against the license
        // allotment (versus one transaction for EACH REQUEST when using the API key). The session
        // key is obtained via an async API call on the Map control itself, so in order to not take
        // dependency on the Bing Maps control here, the key is communicated through an application
        // level resource.
        protected override String _apiKey
        {
            get
            {
                if (Application.Current.Resources.ContainsKey("BingMapsSessionKey"))
                    return Application.Current.Resources["BingMapsSessionKey"] as String;
                else
                    return (Application.Current.Resources.ContainsKey("BingMapsAPIKey")) ?
                        Application.Current.Resources["BingMapsAPIKey"] as String :
                        String.Empty;
            }
        }

        /// <summary>
        /// Performs a Bing Maps location query given <paramref name="searchCriteria"/>
        /// </summary>
        /// <param name="searchCriteria">Free form search criteria</param>
        /// <param name="maxResults">Maximum number of results to assign to view model (default is 0 = no throttling)</param>
        /// <param name="tileSize">Size of square static tile (in pixels) centered on location that should be generated (default is 0 = no tile) (</param>
        /// <returns>Status of API call <seealso cref="APIMASH.ApiResponseStatus"/></returns>
        public async Task<APIMASH.ApiResponseStatus> GetLocations(String searchCriteria, Int32 maxResults = 0, Int32 tileSize = 0)
        {
            // clear the results
            BingMapsViewModel.Results.Clear();

            // if search critera is blank don't run query
            if (String.IsNullOrEmpty(searchCriteria) || (searchCriteria.Trim().Length == 0)) return ApiResponseStatus.Default;
            
            // invoke the API
            var apiResponse = await Invoke<BingMapsLocationsModel.RootObject>(
                "http://dev.virtualearth.net/REST/v1/Locations?q={0}&maxResults=20&key={1}",
                Uri.EscapeUriString(searchCriteria),
                this._apiKey);

            // if successful, copy relevant portions from model to the view model
            if (apiResponse.IsSuccessStatusCode)
            {
                // if throttled, you'll get a special header code
                if (apiResponse.Headers.Contains("X-MS-BM-WS-INFO") && apiResponse.Headers.GetValues("X-MS-BM-WS-INFO").First() == "1")
                {
                    apiResponse.SetCustomStatus("Your request could not be handled at this time. Please try again in a few seconds.");
                }
                else
                {
                    // copy the results from the model to the view model
                    BingMapsLocationsModel.PopulateViewModel(apiResponse.DeserializedResponse, BingMapsViewModel.Results, maxResults);

                    // if tiles are requested, make additional calls to retrieve them. Note there is no error checking of the response.
                    // If someting goes awry, a generic map icon image will be returned
                    if (tileSize > 0)
                    {
                        foreach (var location in BingMapsViewModel.Results)
                            await GetStaticTile(location, tileSize);
                    }
                }
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
                        apiResponse.SetCustomStatus("Problem appears to be on Bing's side. Please retry later.", apiResponse.StatusCode);
                        break;
                }
            }

            // return the status information
            return apiResponse as APIMASH.ApiResponseStatus;
        }

        /// <summary>
        /// Gets a static Bing Maps tile image centered on location of interest and scaled to include that location's extent
        /// </summary>
        /// <param name="location">Location for which to generate map image</param>
        /// <param name="tileSize">Dimensions (in pixels) of square map image</param>
        /// <returns>Status of API call <seealso cref="APIMASH.ApiResponseStatus"/></returns>
        public async Task<APIMASH.ApiResponseStatus> GetStaticTile(BingMapsLocationViewModel location, Int32 tileSize)
        {
            // determine the best zoom level for this particular image based on the bounding box of the location as returned by the
            // the Bing Maps location query
            var zoomLevel = MapUtilities.OptimalZoomLevel(location.Position.Latitude, location.Extent, tileSize);

            // invoke the API
            var apiResponse = await Invoke<BitmapImage>(
                "http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/{0},{1}/{2}?mapSize={3},{3}&key={4}",
                location.Position.Latitude, location.Position.Longitude, zoomLevel, tileSize,
                this._apiKey);

            // set a generic tile image to be returned if something goes wrong
            location.Tile = new BitmapImage(new Uri("ms-appx:///APIMASH_APIs/Assets/genericTile.png"));

            // if successful
            if (apiResponse.IsSuccessStatusCode)
            {
                if (apiResponse.Headers.Contains("X-MS-BM-WS-INFO") && apiResponse.Headers.GetValues("X-MS-BM-WS-INFO").First() == "1")
                {
                    apiResponse.SetCustomStatus("Your request could not be handled at this time. Please try again in a few seconds.");
                }
                else
                {
                    location.Tile = apiResponse.DeserializedResponse;
                }
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
                        apiResponse.SetCustomStatus("Problem appears to be on Bing's side. Please retry later.", apiResponse.StatusCode);
                        break;
                }
            }

            // return the status information
            return apiResponse as APIMASH.ApiResponseStatus;
        }
    }
}