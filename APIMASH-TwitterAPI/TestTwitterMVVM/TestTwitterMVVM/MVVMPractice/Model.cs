using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using MVVMPractice.Common;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using System.Net.Http;
using System.Collections;
using System.Net.Http.Headers;
using TwitterOAuth;


namespace MVVMPractice.Model
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
        #endregion

        #region TweetPublicFields
        public User TweetUser
        {
            get { return _tweetUser; }
            set
            {
                _tweetUser = value != _tweetUser ? value : _tweetUser;
                OnPropertyChanged("TweetUser");

            }
        }

        public ImageSource TweetImage
        {
            get { return _tweetImg; }
            set
            {
                _tweetImg = value != _tweetImg ? value : _tweetImg;
                OnPropertyChanged("TweetImage");
            }
        }

        public string TweetText
        {
            get { return _tweetText; }
            set
            {
                _tweetText = value != _tweetText ? value : _tweetText;
                OnPropertyChanged("TweetText");

            }
        }

        public string ScreenName
        {
            get { return _tweetScreenName; }
            set
            {
                _tweetScreenName = value != _tweetScreenName ? value : _tweetScreenName;
                OnPropertyChanged("ScreenName");
            }
        }

        public string CreateDate
        {
            get { return _creationDate; }
            set
            {
                _creationDate = value != _creationDate ? value : _creationDate;
                OnPropertyChanged("CreateDate");
            }
        }

        public string UserID
        {
            get { return _userID; }
            set
            {
                _userID = value != _userID ? value : _userID;
                OnPropertyChanged("UserID");
            }
        }

        public string StatusID
        {
            get { return _statusID; }
            set
            {
                _statusID = value != _statusID ? value : _statusID;
                OnPropertyChanged("StatusID");
            }
        }

        public string LocationCountry
        {
            get { return _locationCountry; }
            set
            {
                _locationCountry = value != _locationCountry ? value : _locationCountry;
                OnPropertyChanged("LocationCountry");
            }
        }

        public string LocationName
        {
            get { return _locationName; }
            set
            {
                _locationName = value != _locationName ? value : _locationName;
                OnPropertyChanged("LocationName");
            }
        }

        public string LocationType
        {
            get { return _locationType; }
            set
            {
                _locationType = value != _locationType ? value : _locationType;
                OnPropertyChanged("LocationType");
            }
        }

        public float[] LocationCoordinates
        {
            get { return _locationCoordinates; }
            set
            {
                _locationCoordinates = value != _locationCoordinates ? value : _locationCoordinates;
                OnPropertyChanged("LocationCoordinates");
            }
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
            {
                _userTwitterID = value != _userTwitterID ? value : _userTwitterID;
                OnPropertyChanged("TwitterID");
            }
        }

        public string ScreenName
        {
            get { return _screenName; }
            set
            {
                _screenName = value != _screenName ? value : _screenName;
                OnPropertyChanged("ScreenName");
            }
        }
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value != _userName ? value : _userName;
                OnPropertyChanged("UserName");
            }
        }

        public string UserDescription
        {
            get { return _userDescription; }
            set
            {
                _userDescription = value != _userDescription ? value : _userDescription;
                OnPropertyChanged("UserDescription");
            }
        }

        public string CreateDate
        {
            get { return _createDate; }
            set
            {
                _createDate = value != _createDate ? value : _createDate;
                OnPropertyChanged("CreateDate");
            }
        }

        public bool ProtectedTweet
        {
            get { return _tweetsProtected; }
            set
            {
                _tweetsProtected = value != _tweetsProtected ? value : _tweetsProtected;
                OnPropertyChanged("ProtectedTweet");
            }
        }

        public ImageSource ProfileImage
        {
            get { return _userProfileImg; }
            set
            {
                _userProfileImg = value != _userProfileImg ? new BitmapImage(new Uri(CommonProperties.BaseUri + _userImgURL)) : _userProfileImg;
                OnPropertyChanged("ProfileImage");
            }

        }

        public int FollowerCount
        {
            get { return _followerCount; }
            set
            {
                _followerCount = value != _followerCount ? value : _followerCount;
                OnPropertyChanged("FollowerCount");
            }
        }

        public int FriendCount
        {
            get { return _friendCount; }
            set
            {
                _friendCount = value != _friendCount ? value : _friendCount;
                OnPropertyChanged("FriendCount");
            }
        }

        public bool Verified
        {
            get { return _verifiedStatus; }
            set
            {
                _verifiedStatus = value != _verifiedStatus ? value : _verifiedStatus;
                OnPropertyChanged("Verified");
            }
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
            set
            {
                _events = value != _events ? value : _events;
                OnPropertyChanged("Events");
            }
        }

        public string TrendName 
        {
            get { return _trendName; }
            set 
            { 
                _trendName = value != _trendName? value : _trendName;
                OnPropertyChanged("TrendName");
            } 
        }

        public string AsOfDate
        {
            get { return _asOfDate; }
            set
            {
                _asOfDate = value != _asOfDate ? value : _asOfDate;
                OnPropertyChanged("AsOfDate");
            }
        }

        public string PromotedContent
        { 
            get { return _promotedContent;}
            set 
            { 
                _promotedContent = value != _promotedContent ? value : _promotedContent;
                OnPropertyChanged("PromotedContent");
            } 
        }

        public string TrendQuery 
        {
            get { return _queryParam;}
            set 
            {
                _queryParam = value != _queryParam ? value : _queryParam ;
                OnPropertyChanged("TrendQuery");
            } 
        }

        public string TrendSearchURL 
        {
            get { return _searchURL;}
            set 
            { 
                _searchURL = value != _searchURL ? value : _searchURL;
                OnPropertyChanged("TrendSearchURL");
            }

        }

        public string TrendLocation
        {
            get { return _trendLocationName; }
            set 
            {
                _trendLocationName = value != _trendLocationName ? value : _trendLocationName;
                OnPropertyChanged("TrendLocation");
            }
        }

        public string TrendLocationID 
        {
            get { return _trendWOEID; }
            set 
            {
                _trendWOEID = value != _trendWOEID ? value : _trendWOEID;
                OnPropertyChanged("TrendLocationID");
            }
        }

        public string CreateDate 
        {
            get { return _createDate; }
            set 
            {
                _createDate = value != _createDate ? value : _createDate;
                OnPropertyChanged("CreateDate");
            }
        }

      #endregion
    }

    public struct TweetAPIConstants
    {
        public const string BearerToken = "https://api.twitter.com/oauth2/token";
        public const string RequestToken = "https://api.twitter.com/oauth/request_token";
        public const string AuthorizeURL = "https://api.twitter.com/oauth/authorize";
        public const string AccessToken = "https://api.twitter.com/oauth/access_token";
        public const string UserTimeLine = "https://api.twitter.com/1.1/statuses/user_timeline.json";
        public const string HomeTimeLine = "https://api.twitter.com/1.1/statuses/home_timeline.json";
        public const string TweetSearch = "https://api.twitter.com/1.1/search/tweets.json";
        public const string TrendSearch = "https://api.twitter.com/1.1/trends/place.json";
        public const string UserFollowers = "https://api.twitter.com/1.1/followers/list.json";
        public const string UserLookup = "https://api.twitter.com/1.1/users/lookup.json";
        public const string APICountString = "count=";
        public const string APIScreenNameString = "screen_name=";
        public const string APIEntityIncludeString = "include_entities=";
        public const string APISearchQueryString = "q=";
        public const string APISearchResultTypeString = "result_type=";
        public const string APITrendLocationWOEIDString = "id=";
        public static string APITrendLocationWOEID = "1"; //Can change this based upon Yahoo WOEID
        public static int APICountKey = 50; //Can change this to your needs max for most calls is 200
        public static string NoCallBack = "oob";
        public static string TweetSignatureMethod = "HMAC-SHA1";
        public static string APIUserID = String.Empty;
        public static string APIUserScreenName = String.Empty;
    }


    public enum TweetAuthType
    {
        AppAuth = 1,
        OAuth = 2
    }


    //public class TweetOAuthOld
    //{
    //    public TweetOAuthOld() 
    //    {
    //        return;
    //    }

    //    public TweetOAuthOld(string ConsumerKey, string ConsumerSecret) 
    //    {
    //        OAuth.OAuthTypes.OAuthConsumerKey = ConsumerKey; //"zIy0siYBOz86NmM9Ecy44g";
    //        OAuth.OAuthTypes.OAuthConsumerSecret = ConsumerSecret; //"xofuiD87yB37Wxgd6sPTSpYVdwrDrlNp8i0D1e4";
    //    }

    //    public string GetTwitterAuth()
    //    {
    //        //First Get Request Token then Get Authorize Token 

    //        TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    //        Random Rand = new Random();
    //        string TwitterUrl = String.Empty, OAuthParams = String.Empty, tweetURL;// = TweetAPIConstants.RequestToken;
    //        //string TwitterSignature, TwitterParameters;
    //        int Nonce = Rand.Next(1000000000); //32-bit signed int

    //        //  
    //        // Compute base signature string and sign it. 
    //        //    This is a common operation that is required for all requests even after the token is obtained. 
    //        //    Parameters need to be sorted in alphabetical order 
    //        //    Keys and values should be  URL Encoded.  
    //        // 

    //        //Use get right now can change to Post
    //        //TwitterUrl = HttpMethodType.GET.ToString() + "&";
    //        TwitterUrl = HttpMethodType.POST.ToString() + "&";
    //        //Build Parameters

    //        //Add the Request Token URL
    //        TwitterUrl += TweetAPIConstants.RequestToken + "?";

    //        //Add OAuth Callback Header and Parameter - //OAuth.OAuthTypes.OAuthCallbackKey = Uri.EscapeUriString(CommonProperties.BaseUri) + Uri.EscapeDataString(TestTwitterMVVM.App.Current.ToString());
    //        //OAuth.OAuthTypes.OAuthCallbackKey = Uri.EscapeUriString(Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri().Host);
    //        //OAuth.OAuthTypes.OAuthCallbackKey = "oob";
    //        TwitterUrl += OAuth.OAuthTypes.OAuthCallbackString + Uri.EscapeDataString(OAuth.OAuthTypes.OAuthCallbackKey) + "&";

    //        //Add OAuth Consumer Key
    //        //OAuth.OAuthTypes.OAuthConsumerKey = "zIy0siYBOz86NmM9Ecy44g";
    //        TwitterUrl += OAuth.OAuthTypes.OAuthConsumerKeyString + Uri.EscapeDataString(OAuth.OAuthTypes.OAuthConsumerKey) + "&";

    //        //Add OAuth Nonce   
    //        OAuth.OAuthTypes.OAuthNonceKey = Nonce.ToString();
    //        TwitterUrl += OAuth.OAuthTypes.OAuthNonceString + Uri.EscapeDataString(OAuth.OAuthTypes.OAuthNonceKey) + "&";

    //        //Add OAuth Signature Method
    //        OAuth.OAuthTypes.OAuthSignatureMethodKey = MacAlgorithmNames.HmacSha1;
    //        TwitterUrl += OAuth.OAuthTypes.OAuthSignatureMethodString + Uri.EscapeDataString(OAuth.OAuthTypes.OAuthSignatureMethodKey) + "&";

    //        //Add OAuth TimeStamp
    //        OAuth.OAuthTypes.OAuthTimestampKey = Convert.ToInt64(SinceEpoch.TotalSeconds).ToString(); //Math.Round(SinceEpoch.TotalSeconds).ToString();
    //        TwitterUrl += OAuth.OAuthTypes.OAuthTimestampString + Uri.EscapeDataString(OAuth.OAuthTypes.OAuthTimestampKey) + "&";

    //        //Add OAuth Version
    //        OAuth.OAuthTypes.OAuthVersionKey = "1.0";
    //        TwitterUrl += OAuth.OAuthTypes.OAuthVersionString + Uri.EscapeDataString(OAuth.OAuthTypes.OAuthVersionKey);

    //        //String SigBaseStringParams = "oauth_callback=" +  Uri.EscapeDataString(TwitterCallbackUrl.Text); 
    //        //SigBaseStringParams += "&" + "oauth_consumer_key=" + TwitterClientID.Text; 
    //        //SigBaseStringParams += "&" + "oauth_nonce=" + Nonce.ToString(); 
    //        //SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1"; 
    //        //SigBaseStringParams += "&" + "oauth_timestamp=" + Math.Round(SinceEpoch.TotalSeconds); 
    //        //SigBaseStringParams += "&" + "oauth_version=1.0"; 
    //        //String SigBaseString = "GET&"; 
    //        //SigBaseString += Uri.EscapeDataString(TwitterUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams); 

    //        //Build OAuth Escaped Data String of ALL Parameters
    //        OAuthParams = OAuth.OAuthTypes.OAuthCallbackString + OAuth.OAuthTypes.OAuthCallbackKey + "&";
    //        OAuthParams += OAuth.OAuthTypes.OAuthConsumerKeyString + OAuth.OAuthTypes.OAuthConsumerKey + "&";
    //        OAuthParams += OAuth.OAuthTypes.OAuthNonceString + OAuth.OAuthTypes.OAuthNonceKey + "&";
    //        OAuthParams += OAuth.OAuthTypes.OAuthSignatureMethodString + OAuth.OAuthTypes.OAuthSignatureMethodKey + "&";
    //        OAuthParams += OAuth.OAuthTypes.OAuthTimestampString + OAuth.OAuthTypes.OAuthTimestampKey + "&";
    //        OAuthParams += OAuth.OAuthTypes.OAuthVersionString + OAuth.OAuthTypes.OAuthVersionKey;


    //        //Get Twitter Consumer Secret & Encrypt 
    //        IBuffer KeyBuffer = CryptographicBuffer.ConvertStringToBinary(OAuth.OAuthTypes.OAuthConsumerSecret + "&", BinaryStringEncoding.Utf8);

    //        //Add Encryption Algorithm Type 
    //        MacAlgorithmProvider HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);

    //        //Create Encrypted Key with Consumer Secret
    //        CryptographicKey MacKey = HmacSha1Provider.CreateKey(KeyBuffer);

    //        //Encrypt and Sign HTTP OAuth String
    //        IBuffer BinaryHttpRequest = CryptographicBuffer.ConvertStringToBinary(Uri.EscapeUriString(TwitterUrl), BinaryStringEncoding.Utf8);
    //        IBuffer SignedHttpRequest = CryptographicEngine.Sign(MacKey, BinaryHttpRequest);

    //        //Convert Encrypted and Signed HTTP Request to String 
    //        string HttpRequestSignature = CryptographicBuffer.EncodeToBase64String(SignedHttpRequest);
    //        OAuth.OAuthTypes.OAuthSignatureKey = HttpRequestSignature;

    //        //Add Signature 
    //        return tweetURL = TweetAPIConstants.RequestToken + "?" + OAuthParams + "&" + OAuth.OAuthTypes.OAuthSignatureString + Uri.EscapeUriString(OAuth.OAuthTypes.OAuthSignatureKey);


    //        //TwitterUrl += "&" + OAuth.OAuthTypes.OAuthSignatureString + Uri.EscapeUriString(HttpRequestSignature);
    //        //string OAuthPostData = 
    //        //String DataToPost = "OAuthoauth_callback=\"" + Uri.EscapeDataString(TwitterCallbackUrl.Text) + "\", oauth_consumer_key=\"" + TwitterClientID.Text + "\", oauth_nonce=\"" + Nonce.ToString() + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"" + Math.Round(SinceEpoch.TotalSeconds) + "\", oauth_version=\"1.0\", oauth_signature=\"" + Uri.EscapeDataString(Signature) + "\""; 

    //        // String TwitterUrl = "https://api.twitter.com/oauth/request_token"; 
    //        //TwitterUrl += "?" + SigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(Signature); 


    //        //Provide URL to get Authorization
    //        //TwitterUrl += "?" + SigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(Signature); 




    //    }

    //    public async void GetTwitterToken(string twitterUrl) 
    //    {
    //        string twitterAuthURL; //
    //        HttpResponseMessage twitterResponse;
    //        string [] responseValues;


    //        HttpClient tweetClient = new HttpClient();
    //        tweetClient.MaxResponseContentBufferSize = int.MaxValue;
    //        tweetClient.DefaultRequestHeaders.ExpectContinue = false;

    //        HttpRequestMessage twitterRequest = new HttpRequestMessage();
    //        responseValues = twitterUrl.Split('?');
    //        twitterRequest.Content = new StringContent(responseValues[1]);
    //        twitterRequest.Method = HttpMethod.Post;
    //        twitterRequest.RequestUri = new Uri(twitterUrl, UriKind.Absolute);
    //        twitterRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

    //        twitterResponse = await tweetClient.SendAsync(twitterRequest);
    //        //responseValues = !String.IsNullOrEmpty(twitterResponse) ? twitterResponse.Split('&') : null;


    //        //IEnumerable<string[]> Values =  responseValues.Select(x => responseValues.Single().Split('='));

    //        //Authorize Token
    //        OAuth.OAuthTypes.OAuthCallbackKey = "oob";
    //        twitterAuthURL = TweetAPIConstants.AuthorizeURL + 
    //                         OAuth.OAuthTypes.OAuthTokenString + "&" + 
    //                         OAuth.OAuthTypes.OAuthCallbackString + 
    //                         OAuth.OAuthTypes.OAuthCallbackKey;

    //        System.Uri StartUri = new Uri(Uri.EscapeUriString(twitterAuthURL));

    //        //DebugPrint("Navigating to: " + TwitterUrl);

    //        WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None,StartUri);

    //        //String GetResponse = await SendDataAsync(twitterUrl);
    //        //DebugPrint("Received Data: " + GetResponse);

    //        //if (GetResponse != null)
    //        //{
    //        //    String oauth_token = null;
    //        //    String oauth_token_secret = null;
    //        //    String[] keyValPairs = GetResponse.Split('&');

    //        //    for (int i = 0; i < keyValPairs.Length; i++)
    //        //    {
    //        //        String[] splits = keyValPairs[i].Split('=');
    //        //        switch (splits[0])
    //        //        {
    //        //            case "oauth_token":
    //        //                oauth_token = splits[1];
    //        //                break;
    //        //            case "oauth_token_secret":
    //        //                oauth_token_secret = splits[1];
    //        //                break;
    //        //        }
    //        //    }

    //        //}
    //    }
    //}
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