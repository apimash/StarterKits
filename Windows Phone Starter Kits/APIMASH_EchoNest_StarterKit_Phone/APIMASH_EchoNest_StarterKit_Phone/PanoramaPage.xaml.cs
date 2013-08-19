using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using APIMASH_EchoNestLib;
using System.Windows.Data;
using System.Threading;

namespace APIMASH_EchoNest_StarterKit_Phone
{
    public class UrlToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is string && value != null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class PanoramaPage : PhoneApplicationPage
    {
        APIMASH_EchoNest echoNest = new APIMASH_EchoNest();

        public PanoramaPage()
        {
            InitializeComponent();

            this.DataContext = echoNest;
            try
            {
                echoNest.LoadArtistAsync(Globals.ArtistName);
            }
            catch(Exception)
            {

            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems[0] != null)
            {
                var song = e.AddedItems[0] as Song_Bindable;
                if (song.Url != null)
                {
                    soundPlayer.MediaOpened += soundPlayer_MediaOpened;
                    soundPlayer.Source = new Uri(song.Url);
                }
                else
                {
                    soundPlayer.Stop();
                }
            }
        }

        void soundPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            soundPlayer.MediaOpened -= soundPlayer_MediaOpened;
            soundPlayer.Play();
        }
    }
}