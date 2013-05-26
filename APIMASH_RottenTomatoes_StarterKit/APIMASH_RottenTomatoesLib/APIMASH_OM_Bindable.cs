using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

/*
 *
 *  A P I   M A S H 
 *
 * These classes define the bindable object model for Rotten Tomatoes Movie Data
*/

namespace APIMASH_RottenTomatoesLib
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
    public class MovieReviewItem : APIMASH_OM_Bindable
    {
        public MovieReviewItem(
            string critic, 
            string date, 
            string freshness, 
            string publication, 
            string quote,
            string link)
        {
            this._critic = critic;
            this._date = date;
            this._freshness = freshness;
            this._publication = publication;
            this._quote = quote;
            this._link = link;
        }

        private string _critic = string.Empty;
        public string Critic
        {
            get { return this._critic; }
            set { this.SetProperty(ref this._critic, value); }
        }

        private string _date = string.Empty;
        public string Date
        {
            get { return this._date; }
            set { this.SetProperty(ref this._date, value); }
        }

        private string _freshness = string.Empty;
        public string Freshness
        {
            get { return this._freshness; }
            set { this.SetProperty(ref this._freshness, value); }
        }

        private string _publication = string.Empty;
        public string Publication
        {
            get { return this._publication; }
            set { this.SetProperty(ref this._publication, value); }
        }

        private string _quote = string.Empty;
        public string Quote
        {
            get { return this._quote; }
            set { this.SetProperty(ref this._quote, value); }
        }

        private string _link = string.Empty;
        public string Link
        {
            get { return this._link; }
            set { this.SetProperty(ref this._link, value); }
        }
    }

    [Windows.Foundation.Metadata.WebHostHidden]
    public class MovieReviewGroup
    {
        private ObservableCollection<MovieReviewItem> _items;

        public MovieReviewGroup()
        {
            _items = new ObservableCollection<MovieReviewItem>();
        }

        public ObservableCollection<MovieReviewItem> Items
        {
            get { return this._items; }
        }

        public void Copy(MovieReviews movieReviews)
        {
            foreach (var mi in movieReviews.Reviews.Select(r => new MovieReviewItem(
                    r.Critic,
                    r.Date,
                    r.Freshness,
                    r.Publication,
                    r.Quote,
                    r.Link.Review)))
{
                this._items.Add(mi);
            }
        }
    }

    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class MovieItemCommom : APIMASH_OM_Bindable
    {
        private static readonly Uri _baseUri = new Uri("ms-appx:///");

        protected MovieItemCommom(string uniqueId, string title, string rating, string audienceScore, string criticsScore, string clips, string reviews, string cast, string imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._rating = rating;
            this._audienceScore = audienceScore;
            this._criticsScore = criticsScore;
            this._clips = clips;
            this._reviews = reviews;
            this._cast = cast;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _rating;

        public string Rating
        {
            get { return _rating; }
            set { this.SetProperty(ref this._rating, value); }
        }

        private string _audienceScore;
        public string AudienceScore
        {
            get { return _audienceScore; }
            set { this.SetProperty(ref this._audienceScore, value); }
        }

        private string _criticsScore;
        public string CriticsScore
        {
            get { return _criticsScore; }
            set { this.SetProperty(ref this._criticsScore, value); }
        }

        private string _clips;
        public string Clips
        {
            get { return _clips; }
            set { this.SetProperty(ref this._clips, value); }
        }

        private string _reviews;
        public string Reviews
        {
            get { return _reviews; }
            set { this.SetProperty(ref this._reviews, value); }
        }
        
        private string _cast;
        public string Cast
        {
            get { return _cast; }
            set { this.SetProperty(ref this._cast, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private string _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(MovieItemCommom._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(string path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class MovieGroupCommom : APIMASH_OM_Bindable
    {
        private static readonly Uri _baseUri = new Uri("ms-appx:///");

        protected MovieGroupCommom(String uniqueId, String title, String imagePath)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(MovieGroupCommom._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public class MovieItem : MovieItemCommom
    {
        public MovieItem(string uniqueId, string title, string rating, string audienceScore, string criticsScore, string clips, string reviews, string cast, string imagePath, string description, MovieGroup movieGroup)
            : base(uniqueId, title, rating, audienceScore, criticsScore, clips, reviews, cast, imagePath, description)
        {
           this._movieGroup = movieGroup;
        }

        private MovieGroup _movieGroup;
        public MovieGroup MovieGroup
        {
            get { return this._movieGroup; }
            set { this.SetProperty(ref this._movieGroup, value); }
        }
    }

    public class MovieGroup : MovieGroupCommom
    {
        public MovieGroup(String uniqueId, String title, String imagePath)
            : base(uniqueId, title, imagePath)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < Items.Count)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > Items.Count)
                        {
                            TopItems.RemoveAt(Items.Count);
                        }
                    }
                    break;
            }
        }

        private ObservableCollection<MovieItem> _items = new ObservableCollection<MovieItem>();
        public ObservableCollection<MovieItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<MovieItem> _topItem = new ObservableCollection<MovieItem>();
        public ObservableCollection<MovieItem> TopItems
        {
            get { return this._topItem; }
        }
    }
}
