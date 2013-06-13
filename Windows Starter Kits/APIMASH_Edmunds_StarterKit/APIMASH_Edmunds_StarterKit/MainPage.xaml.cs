using System;
using APIMASH_EdmundsLib;
using APIMASHLib;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.System;
using Windows.UI.ApplicationSettings;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_Edmunds_StarterKit
{
    public sealed partial class MainPage : LayoutAwarePage
    {
        private readonly APIMASHInvoke _apiInvokeYearMakeModel;
        private readonly APIMASHInvoke _apiInvokeModelSpecs;
        private readonly APIMASHInvoke _apiInvokePhotoByStyleId;

        public MainPage()
        {
            this.InitializeComponent();
            
            _apiInvokeYearMakeModel = new APIMASHInvoke();
            _apiInvokeModelSpecs = new APIMASHInvoke();
            _apiInvokePhotoByStyleId = new APIMASHInvoke();

            _apiInvokeYearMakeModel.OnResponse += apiInvokeYearMakeModel_OnResponse;
            _apiInvokeModelSpecs.OnResponse += _apiInvokeModelSpecs_OnResponse;
            _apiInvokePhotoByStyleId.OnResponse += _apiInvokePhotoByStyleId_OnResponse;

            // adust the height of the ListView
            var bounds = Window.Current.Bounds;
            ModelList.Height = bounds.Height - 400;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            YearCombo.SelectedIndex = 13;
            var settingsPane = SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested += settingsPane_CommandsRequested;
        }

        void settingsPane_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            // update to supply links to your about, support and privavy policy web pages
            var aboutCmd = new SettingsCommand("About", "About", (x) => Launcher.LaunchUriAsync(new Uri("")));
            var supportCmd = new SettingsCommand("Support", "Support", (x) => Launcher.LaunchUriAsync(new Uri("")));
            var policyCmd = new SettingsCommand("PrivacyPolicy", "Privacy Policy", (x) => Launcher.LaunchUriAsync(new Uri("")));
            
            args.Request.ApplicationCommands.Add(aboutCmd);
            args.Request.ApplicationCommands.Add(supportCmd); 
            args.Request.ApplicationCommands.Add(policyCmd);
        }

        private void InvokeYearMakeModel(string year)
        {
            Sleep(1000); 
            var apiCall = Globals.EDMUNDS_API_FINDBYYEAR + year;
            //var apiCall = Globals.EDMUNDS_API_FINDALL;
            _apiInvokeYearMakeModel.Invoke<MakeCollection>(apiCall);
        }

        async void apiInvokeYearMakeModel_OnResponse(object sender, APIMASHEvent e)
        {
            var response = (MakeCollection)e.Object;

            if (e.Status == APIMASHStatus.SUCCESS)
            {
                APIMASH_EdmundsCarCollection.Copy(response);
                this.DefaultViewModel["Makes"] = APIMASH_EdmundsCarCollection.AllMakes();
                MakeCombo.ItemsSource = APIMASH_EdmundsCarCollection.AllMakes();
                MakeCombo.SelectedIndex = 1;
                ModelList.ItemsSource = APIMASH_EdmundsCarCollection.Cars.All[1].Models;
                ModelList.SelectedIndex = 0;
            }
            else
            {
                var md = new MessageDialog(e.Message, "Error");
                var result = true;
                md.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler((cmd) => result = true)));
                await md.ShowAsync();          
            }
        }

        private void InvokeModelSpecsByMakeModelYear(string make, string model, string year)
        {
            Sleep(1000);

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
                    if (response.ModelSpecs.Length > 0)
                    {
                        if (response.ModelSpecs[0].Styles != null)
                            if (response.ModelSpecs[0].Styles.Length > 0)
                                InvokePhotoById(response.ModelSpecs[0].Styles[0].Id);
                    }
                    else
                    {
                        // this vehicle did not return spec data so display image place holders
                        DisplayPlaceholderImages();
                    }
            }
            else
            {
                // this vehicle did not return spec data so display image place holders
                DisplayPlaceholderImages();
            }
        }

        private void InvokePhotoById(string styleId)
        {
            Sleep(1000);

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
                APIMASH_EdmundsPhotoCollection.Copy(response);

                if (APIMASH_EdmundsPhotoCollection.Photos.All[0].Pictures.Count > 0)
                {
                    var maxPictures = APIMASH_EdmundsPhotoCollection.Photos.All.Count;
                    var totalImages = 0;

                    totalImages = maxPictures < maxImages ? maxPictures : maxImages;

                    for (var i = 0; i < maxImages; i++)
                        images[i] = new BitmapImage(new Uri("ms-appx:///Assets/Car.png")); 
                    
                    for (var i = 0; i < totalImages; i++)
                        images[i] = new BitmapImage(new Uri(APIMASH_EdmundsPhotoCollection.Photos.All[i].Pictures[0]));

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
                    DisplayPlaceholderImages();
                }
            }
            else
            {
                DisplayPlaceholderImages();
            }
        }

        private void DisplayPlaceholderImages()
        {
            const int maxImages = 9;
            var images = new ImageSource[maxImages];

            for (var i = 0; i < maxImages; i++)
                images[i] = new BitmapImage(new Uri("ms-appx:///Assets/Car.png"));

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

        private void ModelList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var make = (EdmundsMake)MakeCombo.SelectedItem;
            var year = (ComboBoxItem)YearCombo.SelectedItem;
            if (make != null)
                if (year != null)
                    if (year.Content != null)
                    {
                        if (ModelList.SelectedIndex >= 0)
                            InvokeModelSpecsByMakeModelYear(
                                make.NiceName, 
                                make.Models[ModelList.SelectedIndex].Name,
                                year.Content.ToString());
                    }
        }

        private void MakeCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var em = (EdmundsMake) MakeCombo.SelectedItem;
            if (em == null) return;
            ModelList.ItemsSource = em.Models;
            ModelList.SelectedIndex = 0;
        }

        private void YearCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var year = (ComboBoxItem) YearCombo.SelectedItem;
            if (year != null) InvokeYearMakeModel(year.Content.ToString());
        }

        private void Logo_OnTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri("http://www.edmunds.com"));
        }

        static void Sleep(int ms)
        {
            new System.Threading.ManualResetEvent(false).WaitOne(ms);
        }
    }
}
