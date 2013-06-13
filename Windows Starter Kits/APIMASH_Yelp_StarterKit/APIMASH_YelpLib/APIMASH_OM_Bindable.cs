/*
 * LICENSE: http://opensource.org/licenses/ms-pl 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace APIMASH_YelpLib
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
    public class BusinessGroup
    {
        private ObservableCollection<BusinessItem> _items;

        public BusinessGroup()
        {
            _items = new ObservableCollection<BusinessItem>();
        }

        public ObservableCollection<BusinessItem> Items
        {
            get { return this._items; }
        }

        public void Copy(Business[] businesses)
        {
            foreach (var bi in businesses.Select(b => new BusinessItem(b)))
            {
                this._items.Add(bi);
            }
        }
    }


    public class BusinessItem : APIMASH_OM_Bindable
    {
        public BusinessItem(Business b)
        {
            this.id = b.Id;
            this.name = b.Name;
            this.address1 = b.Address1;
            this.address2 = b.Address2;
            this.address3 = b.Address3;
            this.avg_rating = b.AvgRating;
            this.review_count = b.ReviewCount;
            this.distance = b.Distance;
            this.city = b.City;
            this.state = b.State;
            this.photoUrl = b.PhotoUrl;
            this.ratingImgUrl = b.RatingImgUrl;
        }

        private string id = string.Empty;
        public string Id
        {
            get { return this.id; }
            set { this.SetProperty(ref this.id, value); }
        }

        private string name = string.Empty;
        public string Name
        {
            get { return this.name; }
            set { this.SetProperty(ref this.name, value); }
        }

        private string address1 = string.Empty;
        public string Address1
        {
            get { return this.address1; }
            set { this.SetProperty(ref this.address1, value); }
        }
        private string address2 = string.Empty;
        public string Address2
        {
            get { return this.address2; }
            set { this.SetProperty(ref this.address2, value); }
        }
        private string address3 = string.Empty;
        public string Address3
        {
            get { return this.address3; }
            set { this.SetProperty(ref this.address3, value); }
        }

        private double avg_rating = 0;
        public double AvgRating
        {
            get { return this.avg_rating; }
            set { this.SetProperty(ref this.avg_rating, value); }
        }

        private int review_count = 0;
        public int ReviewCount
        {
            get { return this.review_count; }
            set { this.SetProperty(ref this.review_count, value); }
        }
        private double distance = 0;
        public double Distance
        {
            get { return this.distance; }
            set { this.SetProperty(ref this.distance, value); }
        }

        private string city = string.Empty;
        public string City
        {
            get { return this.city; }
            set { this.SetProperty(ref this.city, value); }
        }
        private string state = string.Empty;
        public string State
        {
            get { return this.state; }
            set { this.SetProperty(ref this.state, value); }
        }

        private string photoUrl = string.Empty;
        public string PhotoUrl
        {
            get { return this.photoUrl; }
            set { this.SetProperty(ref this.photoUrl, value); }
        }

        private string ratingImgUrl = string.Empty;
        public string RatingImgUrl
        {
            get { return this.ratingImgUrl; }
            set { this.SetProperty(ref this.ratingImgUrl, value); }
        }
    }
}