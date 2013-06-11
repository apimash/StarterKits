using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;
using APIMASH_EdmundsLib;
using Microsoft.Phone.Controls;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_Edmunds_StarterKit_Phone
{
    public partial class ModelPage : PhoneApplicationPage
    {
        private string _make;

        public ModelPage()
        {
            InitializeComponent();
        }

        // When page is navigated to set data context
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext == null)
            {
                var makeId = string.Empty;
                if (NavigationContext.QueryString.TryGetValue("makeId", out makeId))
                {
                    var makeViewModel = LookupMake(System.Convert.ToInt32(makeId));
                    _make = makeViewModel.NiceName;
                    DataContext = makeViewModel;
                }
            }
        }

        public static EdmundsMake LookupMake(int uniqueId)
        {
            var matches =  App.MakeModelViewModel.Makes.Where(x => x.Id == uniqueId);
            return matches.First();
        }

        private void ModelLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (ModelLongListSelector.SelectedItem == null)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/PhotoPage.xaml?make=" + _make + "&model=" + (ModelLongListSelector.SelectedItem as EdmundsModel).Name, UriKind.Relative));

            // Reset selected item to null (no selection)
            ModelLongListSelector.SelectedItem = null;
        }
    }
}