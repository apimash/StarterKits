using APIMASHLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_EchoNestLib
{
    public sealed class APIMASH_EchoNest
    {
        #region Member Variables
//#error Change the EchoNest Developer Key and then remove this error
        const string APIKEY = "<YOUR DEVELOPER API KEY>";
        readonly APIMASHInvoke apiInvoke = new APIMASHInvoke();
        #endregion Member Variables

        public APIMASH_EchoNest()
        {
            apiInvoke.OnResponse += apiInvoke_OnResponse;
            HubImage = new Image_Bindable();
        }

        #region Artist Methods
        private Artist_Bindable _artist = new Artist_Bindable();
        public Artist_Bindable Artist 
        {
            get { return _artist; }
            internal set { _artist = value; }
        }

        public void Copy(Artist response)
        {
            try
            {
                response.Copy(this.Artist);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
        #endregion Artist Methods

        #region Image Methods
        private ObservableCollection<Image_Bindable> _allImages = new ObservableCollection<Image_Bindable>();
        public ObservableCollection<Image_Bindable> AllImages
        {
            get { return this._allImages; }
        }

        public Image_Bindable HubImage 
        {
            get; internal set; 
        }

        public void Copy(IEnumerable<Image> response)
        {
            try
            {
                this._allImages.Clear();
                if (response.Count() > 0)
                {
                    foreach (var item in response)
                    {
                        var newitem = new Image_Bindable();
                        item.Copy(newitem);
                        this._allImages.Add(newitem);
                    }
                    HubImage.Url = _allImages.First().Url;
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
        #endregion Image Methods

        #region News Methods
        private ObservableCollection<News_Bindable> _allNews = new ObservableCollection<News_Bindable>();
        public ObservableCollection<News_Bindable> AllNews
        {
            get { return this._allNews; }
        }

        public void Copy(IEnumerable<News> response)
        {
            try
            {
                this._allNews.Clear();
                if (response.Count() > 0)
                {
                    foreach (var item in response)
                    {
                        var newitem = new News_Bindable();
                        item.Copy(newitem);
                        this._allNews.Add(newitem);
                    }
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
        #endregion News Methods

        #region ArtistSongs Methods
        private ObservableCollection<ArtistSong_Bindable> _allArtistSongs = new ObservableCollection<ArtistSong_Bindable>();
        public ObservableCollection<ArtistSong_Bindable> AllArtistSongs
        {
            get { return this._allArtistSongs; }
        }

        public void Copy(IEnumerable<ArtistSong> response)
        {
            try
            {
                this._allArtistSongs.Clear();
                if (response.Count() > 0)
                {
                    foreach (var item in response)
                    {
                        var newitem = new ArtistSong_Bindable();
                        item.Copy(newitem);
                        this._allArtistSongs.Add(newitem);
                    }
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
        #endregion ArtistSongs Methods

        #region Songs Methods
        private ObservableCollection<Song_Bindable> _allSongs = new ObservableCollection<Song_Bindable>();
        public ObservableCollection<Song_Bindable> AllSongs
        {
            get { return this._allSongs; }
        }

        public void Copy(IEnumerable<Song> response)
        {
            try
            {
                this._allSongs.Clear();
                if (response.Count() > 0)
                {
                    foreach (var item in response)
                    {
                        var newitem = new Song_Bindable();
                        item.Copy(newitem);
                        this._allSongs.Add(newitem);
                    }
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
        #endregion ArtistSongs Methods

        async public void LoadArtistAsync(string name)
        {
            string apiCall = @"http://developer.echonest.com/api/v4/artist/search?api_key=" + APIKEY + @"&format=json&results=1&name=" + name;
            apiInvoke.Invoke<ArtistRootobject>(apiCall);
        }

        void apiInvoke_OnResponse(object sender, APIMASHEvent e)
        {
            try
            {
                if (e.Object is ArtistRootobject)
                {
                    apiInvoke_OnArtistResponse(sender, e);
                }
                else if (e.Object is ImagesRootobject)
                {
                    apiInvoke_OnImagesResponse(sender, e);
                }
                else if (e.Object is NewsRootobject)
                {
                    apiInvoke_OnNewsResponse(sender, e);
                }
                else if (e.Object is SongsRootobject)
                {
                    apiInvoke_OnArtistSongsResponse(sender, e);
                }
            }
            catch(Exception)
            {

            }
        }

        async private void apiInvoke_OnArtistResponse(object sender, APIMASHEvent e)
        {
            if (e.Status == APIMASHStatus.SUCCESS)
            {
                var artist = (ArtistRootobject)e.Object;

                if (artist.response.artists != null && artist.response.artists.Count() > 0)
                {
                    Copy(artist.response.artists.First());
                }
                else
                {
                    throw new Exception("This artist was not found.");
                }

                var imagerequest = @"http://developer.echonest.com/api/v4/artist/images?api_key=" + APIKEY + @"&format=json&results=15&start=0&id=" + artist.response.artists.First().id;
                apiInvoke.Invoke<ImagesRootobject>(imagerequest);

            }
            else
            {
                throw new Exception(e.Message);
            }
        }

        async private void apiInvoke_OnImagesResponse(object sender, APIMASHEvent e)
        {
            if (e.Status == APIMASHStatus.SUCCESS)
            {
                var images = (ImagesRootobject)e.Object;
                Copy(images.response.images);

                var newsrequest = @"http://developer.echonest.com/api/v4/artist/news?api_key=" + APIKEY + @"&format=json&results=10&start=0&id=" + this.Artist.Id;
                apiInvoke.Invoke<NewsRootobject>(newsrequest);
            }
            else
            {
                throw new Exception(e.Message);
            }
        }
        async private void apiInvoke_OnNewsResponse(object sender, APIMASHEvent e)
        {
            if (e.Status == APIMASHStatus.SUCCESS)
            {
                var images = (NewsRootobject)e.Object;
                Copy(images.response.news);

                var songrequest = @"http://developer.echonest.com/api/v4/song/search?api_key=" + APIKEY + @"&format=json&results=50&start=0&bucket=id:7digital-US&bucket=audio_summary&bucket=tracks&artist=" + this.Artist.Name;
                apiInvoke.Invoke<SongsRootobject>(songrequest);
            }
            else
            {
                throw new Exception(e.Message);
            }
        }

        async private void apiInvoke_OnArtistSongsResponse(object sender, APIMASHEvent e)
        {
            if (e.Status == APIMASHStatus.SUCCESS)
            {
                var songs = (SongsRootobject)e.Object;
                Copy(songs.response.songs);
            }
            else
            {
                throw new Exception(e.Message);
            }
        }
        


    }
}
