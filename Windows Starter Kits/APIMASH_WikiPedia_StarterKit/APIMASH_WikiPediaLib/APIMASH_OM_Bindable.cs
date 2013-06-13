// LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt   <== yes, there's a space in it, dont ask....
// APIMash - http://bit.ly/apimash
// Joe Healy / jhealy@microsoft.com / josephehealy@hotmail.com / @devfish

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace APIMASH_WikiPediaLib
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public class APIMASH_OM_Bindable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
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
    
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class geonameItemAbstract : APIMASH_OM_Bindable
    {
        public geonameItemAbstract()
        {
            // intentionally left blank
        }
        public geonameItemAbstract(string summary, float distance, int rank, string title, string wikipediaUrl, int elevation, string countryCode,
                                float lat, float lng, string lang, int geoNameId, string thumbnailImg, string feature)
        {
            this._summary = summary;
            this._distance = distance;
            this._rank = rank;
            this._title = title;
            this._wikipediaUrl = wikipediaUrl;
            this._elevation = elevation;
            this._countryCode = countryCode;
            this._lat = lat;
            this._lng = lng;
            this._lang = lang;
            this._geoNameId = geoNameId;
            this._thumbnailImg = thumbnailImg;
            this._feature = feature;
        }

        private string _feature;
        public string feature
        {
            get { return _feature; }
            set { this.SetProperty(ref this._feature, value); }
        }

        private int _geoNameId;
        public int geoNameId
        {
            get { return _geoNameId; }
            set { this.SetProperty(ref this._geoNameId, value); }
        }

        private string _summary;
        public string summary 
        { 
            get { return _summary;}
            set { this.SetProperty(ref this._summary, value); }
        }

        private string _thumbnailImg;
        public string thumbnailImg 
        {
            get { return _thumbnailImg; }
            set { this.SetProperty(ref this._thumbnailImg, value); }
        }

        private float _distance;
        public float distance 
        {
            get { return _distance; }
            set { this.SetProperty(ref this._distance, value); }
        }

        private int _rank;
        public int rank {
            get { return _rank; }
            set { this.SetProperty(ref this._rank, value); }
        }

        private string _title;
        public string title 
        {
            get { return _title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _wikipediaUrl;
        public string wikipediaUrl 
        {
            get { return _wikipediaUrl; }
            set { this.SetProperty(ref this._wikipediaUrl, value); } 
        }

        private int _elevation;
        public int elevation 
        {
            get { return _elevation; }
            set { this.SetProperty(ref this._elevation, value); }
        }

        private string _countryCode = "n/a";
        public string countryCode 
        { 
            get {return _countryCode;} 
            set { this.SetProperty( ref this._countryCode, value);} 
        }

        private float _lng;
        public float lng 
        { 
            get {return _lng;} 
            set { this.SetProperty( ref this._lng, value );} 
        }

        private string _lang;
        public string lang 
        { 
            get { return _lang;} 
            set { this.SetProperty( ref this._lang, value );} 
        }
        private float _lat;
        public float lat 
        { 
            get { return _lat; } 
            set { this.SetProperty( ref this._lat, value);} 
        }

    } // class

    public class geonameItem : geonameItemAbstract
    {
        public geonameItem(string summary, float distance, int rank, string title, string wikipediaUrl, int elevation, string countryCode,
                        float lat, float lng, string lang, int geoNameId, string thumbnailImg, string feature)
            : base(summary, distance, rank, title, wikipediaUrl, elevation, countryCode, lat, lng, lang, geoNameId, thumbnailImg, feature)
        {
            // intentionally left blank
        }

        // copy from base to bindable
        public geonameItem( APIMASH_WikiPediaLib.geoname gn)
        {
            this.summary = gn.summary;
            this.distance = gn.distance;
            this.rank = gn.rank;
            this.title = gn.title;
            this.wikipediaUrl = gn.wikipediaUrl;
            this.elevation = gn.elevation;
            this.countryCode = gn.countryCode;
            this.lat = gn.lat;
            this.lng = gn.lng;
            this.lang = gn.lang;
            this.geoNameId = gn.geoNameId;
            this.thumbnailImg = gn.thumbnailImg;
            this.feature = gn.feature;
        }
    }

    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class geonamesCollectionBase : APIMASH_OM_Bindable
    {
        public geonamesCollectionBase(){}
    }

    public class geonamesCollection : geonamesCollectionBase
    {
        public geonamesCollection()
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<geonameItem> _items = new ObservableCollection<geonameItem>();
        public ObservableCollection<geonameItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<geonameItem> _topItem = new ObservableCollection<geonameItem>();
        public ObservableCollection<geonameItem> TopItems
        {
            get { return this._topItem; }
        }
    }


}







