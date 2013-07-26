/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace APIMASH_MeetupMaps_StarterKit.ViewModels
{
    public class MeetupViewModel : INotifyPropertyChanged
    {
        private string _id;
        /// <summary>
        /// Sample ViewModel property; this property is used to identify the object.
        /// </summary>
        /// <returns></returns>
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private string _meetupId;
        /// <summary>
        /// Meetup ID property; this property is used to identify the object.
        /// </summary>
        /// <returns></returns>
        public string MeetupID
        {
            get
            {
                return _meetupId;
            }
            set
            {
                if (value != _meetupId)
                {
                    _meetupId = value;
                    NotifyPropertyChanged("MeetupID");
                }
            }
        }

        private string _name;
        /// <summary>
        /// Name property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private string _url;
        /// <summary>
        /// Name property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (value != _url)
                {
                    _url = value;
                    NotifyPropertyChanged("Url");
                }
            }
        }

        private string _description;
        /// <summary>
        /// Description property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        private string _cityState;
        /// <summary>
        /// CityState property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string CityState
        {
            get
            {
                return _cityState;
            }
            set
            {
                if (value != _cityState)
                {
                    _cityState = value;
                    NotifyPropertyChanged("CityState");
                }
            }
        }

        private string _latitude;
        /// <summary>
        /// Latitude property;
        /// </summary>
        public string Latitude 
        { 
            get
            {
                return _latitude;
            }
            set 
            {
                if (value != _latitude)
                {
                    _latitude = value;
                    NotifyPropertyChanged("Latitude");
                }
            }
        }

        private string _longitude;
        /// <summary>
        /// Latitude property;
        /// </summary>
        public string Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                if (value != _longitude)
                {
                    _longitude = value;
                    NotifyPropertyChanged("Longitude");
                }
            }
        }

        private string _latlong;
        /// <summary>
        /// LatLong property; Used to simplify map databinding
        /// </summary>
        public string LatLong
        {
            get
            {
                return _latlong;
            }
            set
            {
                if (value != _latlong)
                {
                    _latlong = value;
                    NotifyPropertyChanged("LatLong");
                }
            }
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