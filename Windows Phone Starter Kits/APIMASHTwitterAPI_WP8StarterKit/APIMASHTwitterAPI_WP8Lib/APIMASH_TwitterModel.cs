using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterAPIWinPhone8Lib.APICommon;
using TwitterAPIWinPhone8Lib.TwitterOAuth;
using System.Windows.Media;

namespace TwitterAPIWinPhone8Lib.Model
{

    public class Tweet : BindableBase
    {
        #region TweetPrivateFields
        private User _tweetUser;
        private string _tweetScreenName;
        private string _tweetText;
        private string _creationDate;
        private string _tweetID;
        private string _userID;
        private string _statusID;
        private float[] _locationCoordinates;
        private string _locationCountry;
        private string _locationName;
        private string _locationType;
        private ImageSource _tweetImg;
        private string _tweetImgURL;
        #endregion

        #region TweetPublicFields
        public User TweetUser
        {
            get { return _tweetUser; }
            set { SetProperty<User>(ref _tweetUser, value); }
        }

        public ImageSource TweetImage
        {
            get { return _tweetImg; }
            set { SetProperty<ImageSource>(ref _tweetImg, value); }
        }

        public string TweetImageUrl
        {
            get { return _tweetImgURL; }
            set { SetProperty<string>(ref _tweetImgURL, value); }
        }

        public string TweetText
        {
            get { return _tweetText; }
            set { SetProperty<string>(ref _tweetText, value); }
        }

        public string ScreenName
        {
            get { return _tweetScreenName; }
            set { SetProperty<string>(ref _tweetScreenName, value); }
        }

        public string CreateDate
        {
            get { return _creationDate; }
            set { SetProperty<string>(ref _creationDate, value); }
        }

        public string UserID
        {
            get { return _userID; }
            set { SetProperty<string>(ref _userID, value); }
        }

        public string StatusID
        {
            get { return _statusID; }
            set { SetProperty<string>(ref _statusID, value); }
        }

        public string LocationCountry
        {
            get { return _locationCountry; }
            set { SetProperty<string>(ref _locationCountry, value); }
        }

        public string LocationName
        {
            get { return _locationName; }
            set { SetProperty<string>(ref _locationName, value); }
        }

        public string LocationType
        {
            get { return _locationType; }
            set { SetProperty<string>(ref _locationType, value); }
        }

        public float[] LocationCoordinates
        {
            get { return _locationCoordinates; }
            set { SetProperty<float[]>(ref _locationCoordinates, value); }
        }

        #endregion
    }

    public class User : BindableBase
    {
        #region UserPrivateFields
        private string _userTwitterID;
        private string _screenName;
        private string _userName;
        private string _userDescription;
        private string _createDate;
        private bool _tweetsProtected;
        private ImageSource _userProfileImg;
        private int _followerCount;
        private int _friendCount;
        private bool _verifiedStatus;
        private string _userImgURL;
        #endregion

        #region UserPublicFields
        public string TwitterID
        {
            get { return _userTwitterID; }
            set
            { SetProperty<string>(ref _userTwitterID, value); }
        }

        public string ScreenName
        {
            get { return _screenName; }
            set
            { SetProperty<string>(ref _screenName, value); }
        }

        public string UserName
        {
            get { return _userName; }
            set { SetProperty<string>(ref _userName, value); }
        }

        public string UserDescription
        {
            get { return _userDescription; }
            set { SetProperty<string>(ref _userDescription, value); }
        }

        public string CreateDate
        {
            get { return _createDate; }
            set { SetProperty<string>(ref _createDate, value); }
        }

        public bool ProtectedTweet
        {
            get { return _tweetsProtected; }
            set
            { SetProperty<bool>(ref _tweetsProtected, value); }
        }

        public ImageSource ProfileImage
        {
            get { return _userProfileImg; }
            set
            { SetProperty<ImageSource>(ref _userProfileImg, value); }
        }

        public string ProfileImageUrl
        {
            get { return _userImgURL; }
            set { SetProperty<string>(ref _userImgURL, value); }
        }

        public int FollowerCount
        {
            get { return _followerCount; }
            set
            { SetProperty<int>(ref _followerCount, value); }
        }

        public int FriendCount
        {
            get { return _friendCount; }
            set { SetProperty<int>(ref _friendCount, value); }
        }

        public bool Verified
        {
            get { return _verifiedStatus; }
            set { SetProperty<bool>(ref _verifiedStatus, value); }
        }

        #endregion
    }

    public class Trend : BindableBase
    {
        #region TrendPrivateFields
        private string _asOfDate;
        private string _events;
        private string _trendName;
        private string _promotedContent;
        private string _queryParam;
        private string _searchURL;
        private string _trendLocationName;
        private string _trendWOEID;
        private string _createDate;
        #endregion

        #region TrendPublicFields
        public string Events
        {
            get { return _events; }
            set { SetProperty<string>(ref _events, value); }
        }

        public string TrendName
        {
            get { return _trendName; }
            set { SetProperty<string>(ref _trendName, value); }
        }

        public string AsOfDate
        {
            get { return _asOfDate; }
            set { SetProperty<string>(ref _asOfDate, value); }
        }

        public string PromotedContent
        {
            get { return _promotedContent; }
            set { SetProperty<string>(ref _promotedContent, value); }
        }

        public string TrendQuery
        {
            get { return _queryParam; }
            set { SetProperty<string>(ref _queryParam, value); }
        }

        public string TrendSearchURL
        {
            get { return _searchURL; }
            set { SetProperty<string>(ref _searchURL, value); }

        }

        public string TrendLocation
        {
            get { return _trendLocationName; }
            set { SetProperty<string>(ref _trendLocationName, value); }
        }

        public string TrendLocationID
        {
            get { return _trendWOEID; }
            set { SetProperty<string>(ref _trendWOEID, value); }
        }

        public string CreateDate
        {
            get { return _createDate; }
            set { SetProperty<string>(ref _createDate, value); }
        }

        #endregion
    }
    public struct TweetAPIConstants
    {
        public const string UserTimeLine = "https://api.twitter.com/1.1/statuses/user_timeline.json";
        public const string HomeTimeLine = "https://api.twitter.com/1.1/statuses/home_timeline.json";
        public const string TweetSearch = "https://api.twitter.com/1.1/search/tweets.json";
        public const string TrendSearch = "https://api.twitter.com/1.1/trends/place.json";
        public const string UserFollowers = "https://api.twitter.com/1.1/followers/list.json";
        public const string MultiUserLookup = "https://api.twitter.com/1.1/users/lookup.json";
        public const string UserLookup = "https://api.twitter.com/1.1/users/show.json"; 
        public const string APICountString = "count=";
        public const string APIScreenNameString = "screen_name=";
        public const string APIEntityIncludeString = "include_entities=";
        public const string APISearchQueryString = "q=";
        public const string APISearchResultTypeString = "result_type=";
        public const string APITrendLocationWOEIDString = "id=";
        public static string APITrendLocationWOEID = "1"; //Can change this based upon Yahoo WOEID
        public static int APICountKey = 75; //Can change this to your needs max for most calls is 200
        public static string APIUserID = String.Empty;
        public static string APIUserScreenName = String.Empty;
    }
}

namespace TweetAPI.Helpers
{
    public class Rootobject
    {
        public TweetHold[] results { get; set; }
    }

    public class TweetHold
    {
        public string created_at { get; set; }
        public long id { get; set; }
        public string id_str { get; set; }
        public string text { get; set; }
        public string source { get; set; }
        public bool truncated { get; set; }
        public object in_reply_to_status_id { get; set; }
        public object in_reply_to_status_id_str { get; set; }
        public object in_reply_to_user_id { get; set; }
        public object in_reply_to_user_id_str { get; set; }
        public object in_reply_to_screen_name { get; set; }
        public TweetUser user { get; set; }
        public object geo { get; set; }
        public object coordinates { get; set; }
        public object place { get; set; }
        public object contributors { get; set; }
        public int retweet_count { get; set; }
        public int favorite_count { get; set; }
        public Entities1 entities { get; set; }
        public bool favorited { get; set; }
        public bool retweeted { get; set; }
        public bool possibly_sensitive { get; set; }
        public string lang { get; set; }
    }

    public class TweetUser
    {
        public int id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public Entities entities { get; set; }
        public bool _protected { get; set; }
        public int followers_count { get; set; }
        public int friends_count { get; set; }
        public int listed_count { get; set; }
        public string created_at { get; set; }
        public int favourites_count { get; set; }
        public string utc_offset { get; set; }
        public string time_zone { get; set; }
        public bool geo_enabled { get; set; }
        public bool verified { get; set; }
        public int statuses_count { get; set; }
        public string lang { get; set; }
        public bool contributors_enabled { get; set; }
        public bool is_translator { get; set; }
        public string profile_background_color { get; set; }
        public string profile_background_image_url { get; set; }
        public string profile_background_image_url_https { get; set; }
        public bool profile_background_tile { get; set; }
        public string profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public string profile_banner_url { get; set; }
        public string profile_link_color { get; set; }
        public string profile_sidebar_border_color { get; set; }
        public string profile_sidebar_fill_color { get; set; }
        public string profile_text_color { get; set; }
        public bool profile_use_background_image { get; set; }
        public bool default_profile { get; set; }
        public bool default_profile_image { get; set; }
        public object following { get; set; }
        public string follow_request_sent { get; set; }
        public object notifications { get; set; }
    }

    public class Entities
    {
        public Url url { get; set; }
        public Description description { get; set; }
    }

    public class Url
    {
        public Url1[] urls { get; set; }
    }

    public class Url1
    {
        public string url { get; set; }
        public object expanded_url { get; set; }
        public int[] indices { get; set; }
    }

    public class Description
    {
        public object[] urls { get; set; }
    }

    public class Entities1
    {
        public object[] hashtags { get; set; }
        public object[] symbols { get; set; }
        public Url2[] urls { get; set; }
        public object[] user_mentions { get; set; }
    }

    public class Url2
    {
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
        public int[] indices { get; set; }
    }


    public class TrendRootobject
    {
        public TrendClass[] Property1 { get; set; }
    }

    public class TrendClass
    {
        public DateTime as_of { get; set; }
        public DateTime created_at { get; set; }
        public Location[] locations { get; set; }
        public TrendTweets[] trends { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
        public int woeid { get; set; }
    }

    public class TrendTweets
    {
        public object events { get; set; }
        public string name { get; set; }
        public object promoted_content { get; set; }
        public string query { get; set; }
        public string url { get; set; }
    }


}


