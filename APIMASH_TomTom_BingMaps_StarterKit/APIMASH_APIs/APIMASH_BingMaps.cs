using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.UI.Xaml;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_BingMaps
{
    /// <summary>
    /// View model class for list of locations returned from a Bing Maps search
    /// </summary>
    public class BingLocationsViewModel
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
        /// Latitude (average of the bounding box, bbox)
        /// </summary>
        public Double Latitude { get; set; }

        /// <summary>
        /// Longitude (average of the bounding box, bbox)
        /// </summary>
        public Double Longitude { get; set; }
    }

    /// <summary>
    /// Class for deserializing raw response data from a Bing Maps location query
    /// </summary>
    public class BingLocationsModel
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
        }

        public class GeocodePoint
        {
            public string type { get; set; }
            public List<double> coordinates { get; set; }
            public string calculationMethod { get; set; }
            public List<string> usageTypes { get; set; }
        }

        [DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1", Name="Location")]
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
        /// Copies the desired portions of the deserialized model data to the view model collection of locations
        /// </summary>
        /// <param name="model">Deserialized result from API call</param>
        /// <param name="maxResults">Maximum number of results to assign to view model (0 = assign all results)</param>
        /// <returns>Indicator of whether items were left out of the view model due to max size restrictions</returns>
        public static Boolean PopulateViewModel(RootObject model, ObservableCollection<BingLocationsViewModel> viewModel, Int32 maxResults = 0)
        {
            // filter criteria
            String[] countryList = { "United States", "Canada" };

            // set up a staging list for applying any filters/max # of items returned, etc.
            var stagingList = new List<BingLocationsViewModel>();

            // clear the view model first
            viewModel.Clear();

            // loop through resource sets (there should only be one)
            foreach (var resourceSet in model.resourceSets)
            {
                // loop through resources in resource set
                foreach (var resource in resourceSet.resources.Where((r) => countryList.Contains(r.address.countryRegion)))
                {

                    // add location to staging list list
                    stagingList.Add(new BingLocationsViewModel()
                    {
                        Address = resource.address.addressLine,
                        City = resource.address.locality,
                        State = resource.address.adminDistrict,
                        Latitude = (resource.bbox[0] + resource.bbox[2]) / 2,
                        Longitude = (resource.bbox[1] + resource.bbox[3]) / 2
                    });
                }
            }

            // apply max count if provided
            var maxResultsExceeded = (maxResults > 0) && (stagingList.Count > maxResults);
            foreach (var s in stagingList.Take(maxResultsExceeded ? maxResults : stagingList.Count))
                viewModel.Add(s);

            return maxResultsExceeded;
        }
    }

    /// <summary>
    /// Wrapper class for Bing Maps API
    /// </summary>
    public sealed class BingApi : APIMASH.ApiBase
    {
        private ObservableCollection<BingLocationsViewModel> _locations =
            new ObservableCollection<BingLocationsViewModel>();
        /// <summary>
        /// List of locations returned by a search (bindable to the UI)
        /// </summary>
        public ObservableCollection<BingLocationsViewModel> Locations
        {
            get { return _locations; }
        }        
        
        /// <summary>
        /// Performs a Bing Maps location query given <paramref name="searchCriteria"/>
        /// </summary>
        /// <param name="searchCriteria">Free form search criteria</param>
        /// <param name="maxResults">Maximum number of results to assign to view model (0 = assign all results)</param>
        /// <returns>Status of API call <seealso cref="APIMASH.ApiResponseStatus"/></returns>
        public async Task<APIMASH.ApiResponseStatus> GetLocations(String searchCriteria, Int32 maxResults = 0)
        {
            // invoke the API
            var apiResponse = await Invoke<BingLocationsModel.RootObject>(
                "http://dev.virtualearth.net/REST/v1/Locations?q={0}&maxResults=20&key={1}",
                Uri.EscapeUriString(searchCriteria),
                this._apiKey);

            // clear the results
            _locations.Clear();

            // if successful, copy relevant portions from model to the view model (and handle special case of server overload)
            if (apiResponse.IsSuccessStatusCode)
            {
                if (apiResponse.Headers.Contains("X-MS-BM-WS-INFO") && apiResponse.Headers.GetValues("X-MS-BM-WS-INFO").First() == "1")
                {
                    apiResponse.StatusCode = HttpStatusCode.Unused;
                    apiResponse.Message = "Bing Maps is running a bit ragged at the moment. Please try again in a few seconds.";
                }
                else
                {
                    BingLocationsModel.PopulateViewModel(apiResponse.DeserializedResponse, _locations, maxResults);
                }
            }
            else
            {
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        apiResponse.Message = "Supplied API key is not valid for this request.";
                        break;

                    case HttpStatusCode.InternalServerError:
                    case HttpStatusCode.ServiceUnavailable:
                        apiResponse.Message = "Problem appears to be on Bing's side. Please retry later.";
                        break;
                }
            }

            // return the status information
            return apiResponse as APIMASH.ApiResponseStatus;
        }

        public BingApi()
        {
            _apiKey = Application.Current.Resources["BingMapsAPIKey"] as String;
        }

        /// <summary>
        /// Update API key to a session key to eliminate billable transactions
        /// </summary>
        /// <param name="key">Session key</param>
        public void SetSessionKey(String key)
        {
            _apiKey = key;
        }
    }
}
