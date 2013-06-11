using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using APIMASHLib;
using APIMASH_EdmundsLib;
using Microsoft.Phone.Controls;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_Edmunds_StarterKit_Phone
{
    public partial class MakePage : PhoneApplicationPage
    {
        private readonly APIMASHInvoke _apiInvokeYearMakeModel;

        public MakePage()
        {
            InitializeComponent();
            _apiInvokeYearMakeModel = new APIMASHInvoke();
            _apiInvokeYearMakeModel.OnResponse += apiInvokeYearMakeModel_OnResponse;
            DataContext = App.MakeModelViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // load make/model data
            if (!App.MakeModelViewModel.IsDataLoaded)
            {
                InvokeYearMakeModel("2013");
            }
        }

        public void InvokeYearMakeModel(string year)
        {
            var apiCall = Globals.EDMUNDS_API_FINDBYYEAR + year;
            _apiInvokeYearMakeModel.Invoke<MakeCollection>(apiCall);
        }

        void apiInvokeYearMakeModel_OnResponse(object sender, APIMASHEvent e)
        {
            var response = (MakeCollection)e.Object;

            if (e.Status == APIMASHStatus.SUCCESS)
            {
                App.MakeModelViewModel.Copy(response);
                App.MakeModelViewModel.IsDataLoaded = true;
            }
            else
            {
                MessageBox.Show(e.Message);
            }
        }

        // Handle selection changed on LongListSelector
        private void MakeLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (MakeLongListSelector.SelectedItem == null)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/ModelPage.xaml?makeId=" + (MakeLongListSelector.SelectedItem as EdmundsMake).Id, UriKind.Relative));

            // Reset selected item to null (no selection)
            MakeLongListSelector.SelectedItem = null;
        }
    }
}