using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using APIMASHLib;
using APIMASH_EdmundsLib;
using Microsoft.Phone.Controls;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_Edmunds_StarterKit_Phone
{
    public partial class PhotoPage : PhoneApplicationPage
    {
        private readonly APIMASHInvoke _apiInvokeModelSpecs;
        private readonly APIMASHInvoke _apiInvokePhotoByStyleId;

        public PhotoPage()
        {
            InitializeComponent();

            _apiInvokeModelSpecs = new APIMASHInvoke();
            _apiInvokePhotoByStyleId = new APIMASHInvoke();

            _apiInvokeModelSpecs.OnResponse += _apiInvokeModelSpecs_OnResponse;
            _apiInvokePhotoByStyleId.OnResponse += _apiInvokePhotoByStyleId_OnResponse;

            ErrMessage.Visibility = Visibility.Collapsed;
        }

        // Load the photo data when this page is navigated to
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var make = string.Empty;
            var model = string.Empty;

            ErrMessage.Visibility = Visibility.Collapsed;

            NavigationContext.QueryString.TryGetValue("make", out make);
            NavigationContext.QueryString.TryGetValue("model", out model);

            // load photos
            InvokeModelSpecsByMakeModelYear(make, model, "2013");
        }

        public void InvokeModelSpecsByMakeModelYear(string make, string model, string year)
        {
            var apiCall =
                Globals.EDMUNDS_API_MODELSPECS_MAKE + make +
                Globals.EDMUNDS_API_MODELSPECS_MODEL + model +
                Globals.EDMUNDS_API_MODELSPECS_YEAR + year +
                Globals.EDMUNDS_API_DEVKEY;
            _apiInvokeModelSpecs.Invoke<ModelSpecCollection>(apiCall);
        }

        async private void _apiInvokeModelSpecs_OnResponse(object sender, APIMASHEvent e)
        {
            var response = (ModelSpecCollection)e.Object;

            if (e.Status == APIMASHStatus.SUCCESS)
            {
                if (response.ModelSpecs != null)
                {
                    if (response.ModelSpecs.Length > 0)
                    {
                        if (response.ModelSpecs[0].Styles != null)
                            if (response.ModelSpecs[0].Styles.Length > 0)
                                InvokePhotoById(response.ModelSpecs[0].Styles[0].Id);
                    }

                    // no model spec
                    DisplayErrMessge();
                }
            }
        }

        private void InvokePhotoById(string styleId)
        {
            // sleeping to avoid calling the API's too often (throttle is 2 times a second)
            Thread.Sleep(500); 
            var apiCall = Globals.EDMUNDS_API_PHOTOS + styleId;
            _apiInvokePhotoByStyleId.Invoke<PhotoCollection>(apiCall);
        }

        async void _apiInvokePhotoByStyleId_OnResponse(object sender, APIMASHEvent e)
        {
            var response = (PhotoCollection)e.Object;

            const int maxImages = 9;
            var images = new ImageSource[maxImages];

            if (e.Status == APIMASHStatus.SUCCESS)
            {
                App.PhotoViewModel.Copy(response);

                if (App.PhotoViewModel.PhotoCollection[0].Pictures.Count > 0)
                {
                    var maxPictures = App.PhotoViewModel.PhotoCollection.Count;
                    var totalImages = 0;

                    totalImages = maxPictures < maxImages ? maxPictures : maxImages;

                    for (var i = 0; i < maxImages; i++)
                        images[i] = new BitmapImage(new Uri("ms-appx:///Assets/Car.png")); 
                    
                    for (var i = 0; i < totalImages; i++)
                        images[i] = new BitmapImage(new Uri(App.PhotoViewModel.PhotoCollection[i].Pictures[0]));

                    ErrMessage.Visibility = Visibility.Collapsed;

                    VehicleImage1.Source = images[0];
                    VehicleImage2.Source = images[1];
                    VehicleImage3.Source = images[2];
                    VehicleImage4.Source = images[3];
                    VehicleImage5.Source = images[4];
                    VehicleImage6.Source = images[5];
                    VehicleImage7.Source = images[6];
                    VehicleImage8.Source = images[7];
                    VehicleImage9.Source = images[8];
                }
                else
                {
                    // no imnages
                    DisplayErrMessge();
                }
            }
            else
            {
                // no images
                DisplayErrMessge();
            }
        }

        private void DisplayErrMessge()
        {
            ErrMessage.Visibility = Visibility;
            ErrMessage.Text = "No Images Available";
        }
    }
}