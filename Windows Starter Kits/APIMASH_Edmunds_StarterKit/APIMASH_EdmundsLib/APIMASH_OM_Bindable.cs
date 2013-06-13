using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

/*
 *
 *  A P I   M A S H 
 *
 * These classes define the bindable object model for Edmunds Vehicle Data
*/

namespace APIMASH_EdmundsLib
{
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

    [Windows.Foundation.Metadata.WebHostHidden]
    public class EdmundsPhoto : BindableBase
    {
        public EdmundsPhoto(string id, string caption, string subType, string shotType)
        {
            this.Id = id;
            this.Caption = caption;
            this.SubType = subType;
            this.ShotType = shotType;
            this.Pictures = new ObservableCollection<string>();
        }

        public ObservableCollection<string> Pictures { get; set; }

        public string _id = string.Empty;

        public string Id
        {
            get { return this._id; }
            set { this.SetProperty(ref this._id, value); }
        }

        private string _caption = string.Empty;

        public string Caption
        {
            get { return this._caption; }
            set { this.SetProperty(ref this._caption, value); }
        }

        private string _subType = string.Empty;

        public string SubType
        {
            get { return this._subType; }
            set { this.SetProperty(ref this._subType, value); }
        }

        private string _shotType = string.Empty;

        public string ShotType
        {
            get { return this._shotType; }
            set { this.SetProperty(ref this._shotType, value); }
        }
    }

    [Windows.Foundation.Metadata.WebHostHidden]
    public class EdmundsMake : BindableBase
    {
        public EdmundsMake(long id, string name, string niceName, string manufacturer)
        {
            this.Id = id;
            this.Name = name;
            this.NiceName = niceName;
            this.Manufacturer = manufacturer;
            Models = new ObservableCollection<EdmundsModel>();
        }

        public ObservableCollection<EdmundsModel> Models { get; private set; }

        public long _id = 0;

        public long Id
        {
            get { return this._id; }
            set { this.SetProperty(ref this._id, value); }
        }

        private string _name = string.Empty;

        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }

        private string _niceName = string.Empty;

        public string NiceName
        {
            get { return this._niceName; }
            set { this.SetProperty(ref this._niceName, value); }
        }

        private string _manufacturer = string.Empty;

        public string Manufacturer
        {
            get { return this._manufacturer; }
            set { this.SetProperty(ref this._manufacturer, value); }
        }
    }

    [Windows.Foundation.Metadata.WebHostHidden]
    public class EdmundsModel : BindableBase
    {
        public EdmundsModel(string link,string id,string name)
        {
            this._link = link;
            this._id = id;
            this._name = name;
        }

        private string _link = string.Empty;

        public string Link
        {
            get { return this._link; }
            set { this.SetProperty(ref this._link, value); }
        }

        private string _id = string.Empty;

        public string Id
        {
            get { return this._id; }
            set { this.SetProperty(ref this._id, value); }
        }

        private string _name = string.Empty;

        public string Name
        {
            get { return this._name; }
            set { this.SetProperty(ref this._name, value); }
        }
    }
}
