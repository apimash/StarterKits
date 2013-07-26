/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using APIMASH_MeetupMaps_StarterKit.Resources;
using System.Device.Location;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Tasks;
using APIMASH_MeetupMaps_StarterKit.Customization;

namespace APIMASH_MeetupMaps_StarterKit
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        int index;

        // Constructor
        public DetailsPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            if (DataContext == null)
            {
                string selectedIndex = "";
                if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedIndex))
                {
                    index = int.Parse(selectedIndex);
                    DataContext = App.ViewModel.Items[index];
                }
            }
        }

        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            double lat = double.Parse(App.ViewModel.Items[index].Latitude);
            double lon = double.Parse(App.ViewModel.Items[index].Longitude);

            MyMap.Center = new GeoCoordinate(lat, lon);
            MyMap.ZoomLevel = 15;

            // Create a small circle to mark the current meetup location.
            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new SolidColorBrush(Colors.Red);
            myCircle.Height = 20;
            myCircle.Width = 20;
            myCircle.Opacity = 50;

            MapOverlay myOverlay = new MapOverlay();
            myOverlay.Content = myCircle;
            myOverlay.PositionOrigin = new Point(0.5, 0.5);
            myOverlay.GeoCoordinate = MyMap.Center;

            MapLayer myLayer = new MapLayer();
            myLayer.Add(myOverlay);
            MyMap.Layers.Add(myLayer);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double lat = double.Parse(App.ViewModel.Items[index].Latitude);
            double lon = double.Parse(App.ViewModel.Items[index].Longitude);

            MapsTask getCoffeeTask = new MapsTask();
            getCoffeeTask.Center = new GeoCoordinate(lat, lon);
            getCoffeeTask.SearchTerm = AppConstants.searchTerm;
            getCoffeeTask.ZoomLevel = 16;
            getCoffeeTask.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WebBrowserTask meetupTask = new WebBrowserTask();

            meetupTask.Uri = new Uri(App.ViewModel.Items[index].Url);
            meetupTask.Show();
        }

    }
}