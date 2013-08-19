using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_EchoNestLib
{

    [DataContract]
    public class ArtistRootobject
    {
        [DataMember(Name = "response")]
        public ArtistResponse response { get; set; }
    }

    [DataContract]
    public class ArtistResponse
    {
        [DataMember(Name = "status")]
        public Status status { get; set; }

        [DataMember(Name = "artists")]
        public Artist[] artists { get; set; }
    }

    [DataContract]
    public class Status
    {
        [DataMember(Name = "status")]
        public string code { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }

        [DataMember(Name = "version")]
        public string version { get; set; }
    }

    [DataContract]
    public class Artist
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "id")]
        public string id { get; set; }

        public void Copy(Artist_Bindable bindable)
        {
            bindable.Name = name;
            bindable.Id = id;
        }
    }


    [DataContract]
    public class ImagesRootobject
    {
        [DataMember(Name = "response")]
        public ImagesResponse response { get; set; }
    }

    [DataContract]
    public class ImagesResponse
    {
        [DataMember(Name = "status")]
        public Status status { get; set; }

        [DataMember(Name = "start")]
        public int start { get; set; }

        [DataMember(Name = "total")]
        public int total { get; set; }

        [DataMember(Name = "images")]
        public Image[] images { get; set; }
    }

    [DataContract]
    public class Image
    {
        [DataMember(Name = "url")]
        public string url { get; set; }
        [DataMember(Name = "license")]
        public License license { get; set; }

        public void Copy(Image_Bindable bindable)
        {
            bindable.Url = url;
        }
    }

    [DataContract]
    public class License
    {
        [DataMember(Name = "url")]
        public string url { get; set; }

        [DataMember(Name = "attribution")]
        public string attribution { get; set; }

        [DataMember(Name = "type")]
        public string type { get; set; }
    }


    [DataContract]
    public class NewsRootobject
    {
        [DataMember(Name = "response")]
        public NewsResponse response { get; set; }
    }

    [DataContract]
    public class NewsResponse
    {
        [DataMember(Name = "status")]
        public Status status { get; set; }

        [DataMember(Name = "start")]
        public int start { get; set; }

        [DataMember(Name = "total")]
        public int total { get; set; }

        [DataMember(Name = "news")]
        public News[] news { get; set; }
    }

    [DataContract]
    public class News
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "url")]
        public string url { get; set; }

        [DataMember(Name = "date_posted")]
        public DateTime date_posted { get; set; }

        [DataMember(Name = "date_found")]
        public DateTime date_found { get; set; }

        [DataMember(Name = "summery")]
        public string summary { get; set; }

        [DataMember(Name = "id")]
        public string id { get; set; }

        public void Copy(News_Bindable bindable)
        {
            bindable.Url = url;
            bindable.Title = name;
            bindable.Summary = summary;
            bindable.Posted = date_found;
        }

    }


    [DataContract]
    public class ArtistSongsRootobject
    {
        [DataMember(Name = "response")]
        public ArtistSongsResponse response { get; set; }
    }

    [DataContract]
    public class ArtistSongsResponse
    {
        [DataMember(Name = "status")]
        public Status status { get; set; }

        [DataMember(Name = "start")]
        public int start { get; set; }

        [DataMember(Name = "total")]
        public int total { get; set; }

        [DataMember(Name = "songs")]
        public ArtistSong[] songs { get; set; }
    }

    [DataContract]
    public class ArtistSong
    {
        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }
        public void Copy(ArtistSong_Bindable bindable)
        {
            bindable.Id = id;
            bindable.Title = title;
        }
    }


    public class SongsRootobject
    {
        public SongsResponse response { get; set; }
    }

    public class SongsResponse
    {
        public Status status { get; set; }
        public Song[] songs { get; set; }
    }

    public class Song
    {
        public string title { get; set; }
        public string artist_name { get; set; }
        public string id { get; set; }
        public Track[] tracks { get; set; }
        public string artist_id { get; set; }
        public string audio_md5 { get; set; }
        public Audio_Summary audio_summary { get; set; }

        public void Copy(Song_Bindable bindable)
        {
            bindable.Id = id;
            bindable.Title = title;
            var firstTrack = tracks != null ? tracks.Where(t => t.preview_url != null) : null;
            bindable.Url = firstTrack != null && firstTrack.Count() > 0 ? firstTrack.First().preview_url : null;
        }
    }

    public class Audio_Summary
    {
        public int key { get; set; }
        public string analysis_url { get; set; }
        public float energy { get; set; }
        public float liveness { get; set; }
        public float tempo { get; set; }
        public float speechiness { get; set; }
        public float acousticness { get; set; }
        public int mode { get; set; }
        public int time_signature { get; set; }
        public float duration { get; set; }
        public float loudness { get; set; }
        public string audio_md5 { get; set; }
        public float valence { get; set; }
        public float danceability { get; set; }
    }

    public class Track
    {
        public string release_image { get; set; }
        public string foreign_release_id { get; set; }
        public string preview_url { get; set; }
        public string catalog { get; set; }
        public string foreign_id { get; set; }
        public string id { get; set; }
    }


}
