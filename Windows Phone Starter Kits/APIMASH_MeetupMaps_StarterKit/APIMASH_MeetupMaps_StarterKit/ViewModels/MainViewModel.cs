/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/


using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using APIMASH_MeetupMaps_StarterKit.Resources;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using APIMASH_MeetupMaps_StarterKit.Customization;

namespace APIMASH_MeetupMaps_StarterKit.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private WebClient _client;

        public MainViewModel()
        {
            _client = new WebClient();
            _client.DownloadStringCompleted += _client_DownloadStringCompleted;
            this.Items = new ObservableCollection<MeetupViewModel>();
        }

        /// <summary>
        /// A collection for MeetupViewModel objects.
        /// </summary>
        public ObservableCollection<MeetupViewModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few MeetupViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {

            AppConstants.meetupUri += "&city=" + AppConstants.meetupCity
                + "&state=" + AppConstants.meetupState
                + "&page=" + AppConstants.maxMeetupsToFind
                + "&key=" + AppConstants.meetupKey
                + "&radius=" + AppConstants.meetupDistance;
            if (AppConstants.meetupKeywords != "")
            {
                AppConstants.meetupUri += "&text=" + AppConstants.meetupKeywords;
            }

            _client.DownloadStringAsync(new Uri(AppConstants.meetupUri));

        }

        void _client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            XElement meetupElements = XElement.Parse(e.Result);

            var meetups =
                from meetup in meetupElements.Descendants("item")
                where meetup.Element("venue") != null
                select new MeetupViewModel
                {
                    MeetupID = meetup.Element("id").Value,
                    Name = meetup.Element("name").Value,
                    Url = meetup.Element("event_url").Value,
                    Description = meetup.Element("description").Value,
                    CityState = meetup.Element("venue").Element("city").Value + ", " + meetup.Element("venue").Element("state").Value,
                    Latitude = meetup.Element("venue").Element("lat").Value,
                    Longitude = meetup.Element("venue").Element("lon").Value,
                    LatLong = meetup.Element("venue").Element("lat").Value + ", " + meetup.Element("venue").Element("lon").Value
                };

            var index = 0;
            foreach (MeetupViewModel meetupItem in meetups)
            {
                meetupItem.ID = index.ToString();
                this.Items.Add(meetupItem);
                index++;
            }

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}