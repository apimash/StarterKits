using System;
using System.IO.IsolatedStorage;
using TomTom_StarterKit_Phone.Common;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace TomTom_StarterKit_Phone
{
    public class SettingsViewModel : BindableBase
    {
        public Boolean UseLocation
        {
            get
            {   
                if (IsolatedStorageSettings.ApplicationSettings.Contains("UseLocation"))
                    _useLocation = (Boolean)IsolatedStorageSettings.ApplicationSettings["UseLocation"];
                else
                    _useLocation = true;
                return _useLocation;
            }
            set
            {
                if (_useLocation != value)
                {
                    IsolatedStorageSettings.ApplicationSettings["UseLocation"] = value;
                    SetProperty(ref _useLocation, value);
                }
            }
        }
        private Boolean _useLocation;

        public Boolean LocationEnabled
        {
            get
            {
                return _locationEnabled;
            }
            set
            {
                SetProperty(ref _locationEnabled, value);     
            }
        }
        private Boolean _locationEnabled;

        public Boolean UseLightMode
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("UseLightMode"))
                    _useLightMode = (Boolean)IsolatedStorageSettings.ApplicationSettings["UseLightMode"];
                else
                    _useLightMode = true;
                return _useLightMode;
            }
            set
            {
                if (_useLightMode != value)
                {
                    IsolatedStorageSettings.ApplicationSettings["UseLightMode"] = value;
                    SetProperty(ref _useLightMode, value);
                }
            }
        }
        private Boolean _useLightMode;
    }
}
