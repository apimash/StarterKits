using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using APIMASH_EchoNest_StarterKit_Phone.Resources;
using APIMASH_EchoNest_StarterKit_Phone.Data;

namespace APIMASH_EchoNest_StarterKit_Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += MainPage_Loaded;
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            sampleArtists.DataContext = sampleDataGroups;
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.ArtistName = searchName.Text;
            searchName.Text = "";
            NavigationService.Navigate(new Uri("/PanoramaPage.xaml", UriKind.Relative));
        }

        private void sampleArtists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems[0] != null)
            {
                var artist = e.AddedItems[0] as SampleDataGroup;
                Globals.ArtistName = artist.UniqueId;
                NavigationService.Navigate(new Uri("/PanoramaPage.xaml", UriKind.Relative));
            }
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}