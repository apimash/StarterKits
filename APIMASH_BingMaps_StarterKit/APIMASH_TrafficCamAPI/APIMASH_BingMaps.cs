using APIMASH;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.UI.Xaml;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_BingMaps
{
    public class BingLocationsViewModel
    {
        public String Address { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
    }

    public class BingLocationsModel
    {
        public RootObject ModelData { get; private set; }
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


        // copy the model to the view model
        public static void PopulateViewModel(RootObject model, ObservableCollection<BingLocationsViewModel> viewModel)
        {
            viewModel.Clear();
            foreach (var resourceSet in model.resourceSets)
            {
                foreach (var resource in resourceSet.resources)
                {
                    viewModel.Add(new BingLocationsViewModel()
                    { 
                        Address = resource.address.addressLine,
                        City = resource.address.locality,
                        State = resource.address.adminDistrict,
                        Latitude = (resource.bbox[0] + resource.bbox[2]) / 2,
                        Longitude = (resource.bbox[1] + resource.bbox[3]) / 2
                    });
                }

            }
        }
    }

    public sealed class BingApi : APIMASH_ApiBase
    {
        public BingApi()
        {
            _apiKey = Application.Current.Resources["BingMapsAPIKey"] as String;
        }

        private ObservableCollection<BingLocationsViewModel> _locations =
            new ObservableCollection<BingLocationsViewModel>();
        public ObservableCollection<BingLocationsViewModel> Locations
        {
            get { return _locations; }
        }
        
        // invoke API to get list of locations matching search criteria
        public async Task GetLocations(String searchCriteria)
        {

            var apiResponse = await Invoke<BingLocationsModel.RootObject>(
                "http://dev.virtualearth.net/REST/v1/Locations?q={0}&maxResults=10&key={1}",
                Uri.EscapeUriString(searchCriteria), 
                this._apiKey);

            if (apiResponse.IsSuccessStatusCode)
            {
                BingLocationsModel.PopulateViewModel(apiResponse.DeserializedResponse, _locations);
            }
            else
            {
                // error information is in apiResponse process as desired :)
            }
        }
    }
}
