using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_TomTom
{
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

    public class TomTomCamerasModel
    {
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

        // copy the model to the view model
        public static void PopulateViewModel(cameras model, ObservableCollection<TomTomCameraViewModel> viewModel)
        {
            Int32 sequence = 0;

            viewModel.Clear();
            if (model.CameraList != null)
                foreach (var camera in
                            (from c in model.CameraList select new TomTomCameraViewModel()
                                {
                                    Sequence = ++sequence,
                                    CameraId = c.cameraId,
                                    Name = c.cameraName,
                                    Orientation = c.orientation,
                                    RefreshRate = c.refreshRate,
                                    Latitude = c.latitude,
                                    Longitude = c.longitude
                                }))
                    viewModel.Add(camera);
        }
    }

    public class BoundingBox
    {
        public Double Top { get; set; }
        public Double Bottom { get; set; }
        public Double Left { get; set; }
        public Double Right { get; set; }

        public BoundingBox(Double t, Double b, Double l, Double r)
        {
            Top = t;
            Bottom = b;
            Left = l;
            Right = r;
        }
    }
    public class TomTomApi : APIMASH.APIMASH_ApiBase
    {
        public TomTomApi()
        {
            _apiKey = Application.Current.Resources["TomTomAPIKey"] as String;
        }

        private ObservableCollection<TomTomCameraViewModel> _cameras =
            new ObservableCollection<TomTomCameraViewModel>();
        public ObservableCollection<TomTomCameraViewModel> Cameras
        {
            get { return _cameras; }
        }

        // invoke API to get list of locations matching search criteria
        public async Task GetCameras(BoundingBox b)
        {
            var apiResponse = await Invoke<TomTomCamerasModel.cameras>(
                "http://api.tomtom.com/trafficcams/boxquery?top={0}&bottom={1}&left={2}&right={3}&format=xml&key={4}",
                b.Top, b.Bottom, b.Left, b.Right,
                this._apiKey);

            if (apiResponse.IsSuccessStatusCode)
            {
                TomTomCamerasModel.PopulateViewModel(apiResponse.DeserializedResponse, _cameras);
            }
            else
            {
                // error information is in apiResponse process as desired :)
            }
        }

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
