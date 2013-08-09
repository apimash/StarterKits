using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_EchoNestLib
{
    #region Bindable Base Class
    [Windows.Foundation.Metadata.WebHostHidden]
    public class BindableBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            if (propertyName != null) this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    #endregion Bindable Base Class

    #region Artist Bindable
    public class Artist_Bindable : BindableBase
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty<string>(ref _name, value); }
        }

        private string _id;

        public string Id
        {
            get { return _id; }
            set { SetProperty<string>(ref _id, value); }
        }
    }
    #endregion Artist Bindable

    #region Image Bindable
    public class Image_Bindable : BindableBase
    {
        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty<string>(ref _url, value); }
        }

        public void FireChanged()
        {
            OnPropertyChanged("Url");
        }
    }
    #endregion Image Bindable

    #region News Bindable
    public class News_Bindable : BindableBase
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty<string>(ref _title, value); }
        }

        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty<string>(ref _url, value); }
        }

        private string _summary;

        public string Summary
        {
            get { return _summary; }
            set { SetProperty<string>(ref _summary, value); }
        }

        private DateTime _posted;

        public DateTime Posted
        {
            get { return _posted; }
            set { SetProperty<DateTime>(ref _posted, value); }
        }
    }
    #endregion News Bindable

    #region ArtistSong Bindable
    public class ArtistSong_Bindable : BindableBase
    {
        private string _id;

        public string Id
        {
            get { return _id; }
            set { SetProperty<string>(ref _id, value); }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty<string>(ref _title, value); }
        }
    }
    #endregion ArtistSong Bindable

    #region Song Bindable
    public class Song_Bindable : BindableBase
    {
        private string _id;

        public string Id
        {
            get { return _id; }
            set { SetProperty<string>(ref _id, value); }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty<string>(ref _title, value); }
        }

        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty<string>(ref _url, value); }
        }
    }
    #endregion Song Bindable

}
