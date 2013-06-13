using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TweetAPI.Helpers;
using TwitterAPIWin8Lib.Model;
using TwitterAPIWin8Lib.TwitterOAuth;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace TwitterAPIWin8Lib.APICalls
{
    
    public class TweetAPICall
    {
        public enum APIType
        {
            Home,
            User,
            Search,
            Trend,
            Follower,
            UserInfo
        }

        private TweetOAuth _tweetAuthObj;
        private Dictionary<string, string> _apiOAuthParams = null;
        private ObservableCollection<Tweet> _tweetReturn;
        private ObservableCollection<User> _usersReturn;
        private User _twitterUserInfo;

        protected ObservableCollection<Tweet> LastTweetReturn
        {
            get { return _tweetReturn; }

        }

        public TweetAPICall()
        {
            _tweetAuthObj = new TweetOAuth();
            _tweetReturn = new ObservableCollection<Tweet>();
        }

        protected Dictionary<string, string> GetAPIOAuthParams()
        {
            Dictionary<string, string> oAuthParams = new Dictionary<string, string>();

            //Get Nonce
            OAuth.OAuthTypes.OAuthNonceKey = _tweetAuthObj.OAuthNonce();
            //Get TimeStamp
            OAuth.OAuthTypes.OAuthTimestampKey = _tweetAuthObj.OAuthTimeStamp(HttpMethod.Get);
            //Get Crypto Method
            OAuth.OAuthTypes.OAuthSignatureMethodKey = TweetAuthConstants.TweetSignatureMethod;

            //Add the needed OAuth parameters for call
            oAuthParams.Add(OAuth.OAuthTypes.OAuthConsumerKeyString, OAuth.OAuthTypes.OAuthConsumerKey);
            oAuthParams.Add(OAuth.OAuthTypes.OAuthNonceString, OAuth.OAuthTypes.OAuthNonceKey);
            oAuthParams.Add(OAuth.OAuthTypes.OAuthSignatureString, OAuth.OAuthTypes.OAuthSignatureKey);
            oAuthParams.Add(OAuth.OAuthTypes.OAuthSignatureMethodString, OAuth.OAuthTypes.OAuthSignatureMethodKey);
            oAuthParams.Add(OAuth.OAuthTypes.OAuthTimestampString, OAuth.OAuthTypes.OAuthTimestampKey);
            oAuthParams.Add(OAuth.OAuthTypes.OAuthTokenString, OAuth.OAuthTypes.OAuthAccessTokenKey);
            oAuthParams.Add(OAuth.OAuthTypes.OAuthVerifierString, OAuth.OAuthTypes.OAuthVerifierKey);
            oAuthParams.Add(OAuth.OAuthTypes.OAuthVersionString, OAuth.OAuthTypes.OAuthVersionKey);

            return oAuthParams;

        }

        protected Dictionary<string, string> GetTwitterAPIParams(APIType APICall, string tweetTarget)
        {
            Dictionary<string, string> tweetParams = new Dictionary<string, string>();

            //Default Count
            tweetParams.Add(TweetAPIConstants.APICountString, TweetAPIConstants.APICountKey.ToString());

            //Get URL appropriate for request needs and Get appropriate parameters
            switch (APICall)
            {
                case APIType.Home:
                    tweetParams.Add(TweetAPIConstants.APIEntityIncludeString, "false");
                    break;

                case APIType.User:
                    tweetParams.Add(TweetAPIConstants.APIEntityIncludeString, "false");
                    tweetParams.Add(TweetAPIConstants.APIScreenNameString, tweetTarget);
                    break;

                case APIType.Search:
                    tweetParams.Add(TweetAPIConstants.APIEntityIncludeString, "false");
                    tweetParams.Add(TweetAPIConstants.APISearchQueryString, tweetTarget);
                    tweetParams.Add(TweetAPIConstants.APISearchResultTypeString, "mixed");
                    break;

                case APIType.Trend:
                    if (tweetTarget == null) { tweetParams.Add(TweetAPIConstants.APITrendLocationWOEIDString, TweetAPIConstants.APITrendLocationWOEID); }
                    else { tweetParams.Add(TweetAPIConstants.APITrendLocationWOEIDString, tweetTarget); }
                    tweetParams.Remove(TweetAPIConstants.APICountString);
                    break;

                case APIType.Follower:
                    tweetParams.Add(TweetAPIConstants.APIEntityIncludeString, "false");
                    tweetParams.Add(TweetAPIConstants.APIScreenNameString, tweetTarget);
                    tweetParams.Remove(TweetAPIConstants.APICountString);
                    break;

                case APIType.UserInfo:
                    break;
                default:
                    throw new ArgumentNullException("APIType", "API Call Type Required; Invalid or Null APIType passed");

            }

            return tweetParams;
        }

        protected string GetTwitterAPIUrl(APIType APICall)
        {
            string tweetUrl;

            //Get URL appropriate for request needs and Get appropriate parameters
            switch (APICall)
            {
                case APIType.Home:
                    tweetUrl = TweetAPIConstants.HomeTimeLine;
                    break;

                case APIType.User:
                    tweetUrl = TweetAPIConstants.UserTimeLine;
                    break;

                case APIType.Search:
                    tweetUrl = TweetAPIConstants.TweetSearch;
                    break;

                case APIType.Trend:
                    tweetUrl = TweetAPIConstants.TrendSearch;
                    break;

                case APIType.Follower:
                    tweetUrl = TweetAPIConstants.UserFollowers;
                    break;

                case APIType.UserInfo:
                    tweetUrl = TweetAPIConstants.UserLookup;
                    break;

                default:
                    throw new ArgumentNullException("APIType", "API Call Type Required; Invalid or Null APIType passed");

            }

            return tweetUrl;
        }

        protected string GetTwitterAPISignatureBase(string apiUrl, Dictionary<string, string> apiReqParams, Dictionary<string, string> apiAuthParams)
        {
            string sigBaseString = string.Empty, sigParameters = string.Empty, oauthSignature = string.Empty;
            Dictionary<string, string> sigBaseParam = new Dictionary<string, string>();

            //Create Signature Base String based upon Twitter OAuth rules

            //Append Http Method to Signature Base String
            sigBaseString += HttpMethod.Get.ToString().ToUpper() + "&";

            //Encode URL and Append to String
            sigBaseString += Uri.EscapeDataString(apiUrl) + "&";

            //Build Combined Parameter Dictionary (Request and OAuth)
            foreach (var parameter in apiReqParams)
            { sigBaseParam.Add(parameter.Key, parameter.Value); }
            foreach (var parameter in apiAuthParams)
            { sigBaseParam.Add(parameter.Key, parameter.Value); }

            //Take Signature KeyPair out and Order alphabetically
            sigBaseParam = sigBaseParam.Where(param => param.Key != OAuth.OAuthTypes.OAuthSignatureString).ToDictionary(p => p.Key, p => p.Value);
            sigBaseParam = sigBaseParam.OrderBy(param => param.Key).ToDictionary(param => param.Key, param => param.Value);

            //Build Parameter String
            sigParameters = sigBaseParam.Select(param => param.Key + param.Value).Aggregate((p1, p2) => p1 + "&" + p2);

            //Encode Parameter string and Append to Signature Base String
            sigBaseString += Uri.EscapeDataString(sigParameters);

            return sigBaseString;
        }

        public string GetSignature(string sigBaseString, string consumerSecretKey, string requestTokenSecretKey = null)
        {
            var signingKey = string.Format("{0}&{1}", consumerSecretKey, !string.IsNullOrEmpty(requestTokenSecretKey) ? requestTokenSecretKey : "");
            IBuffer keyMaterial = CryptographicBuffer.ConvertStringToBinary(signingKey, BinaryStringEncoding.Utf8);
            MacAlgorithmProvider hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey macKey = hmacSha1Provider.CreateKey(keyMaterial);
            IBuffer dataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            IBuffer signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            String signature = CryptographicBuffer.EncodeToBase64String(signatureBuffer);
            return signature;
        }

        public ObservableCollection<Trend> BuildTrendCollection(JArray trendJsonArray)
        {
            ObservableCollection<Trend> trendTweets = new ObservableCollection<Trend>();

            //TrendClass trendHold = trendJsonArray.ToObject<TrendClass>();

            foreach (var jItem in trendJsonArray)
            {
                TrendClass trendHold = jItem.ToObject<TrendClass>();

                foreach (var trend in trendHold.trends)
                {
                    Trend trendItem = new Trend();
                    trendItem.AsOfDate = trendHold.as_of.ToString();
                    trendItem.CreateDate = trendHold.created_at.ToString();
                    trendItem.TrendLocation = trendHold.locations.First().name;
                    trendItem.TrendLocationID = trendHold.locations.First().woeid.ToString();
                    trendItem.TrendName = trend.name;
                    trendItem.PromotedContent = trend.promoted_content != null ? trend.promoted_content.ToString() : String.Empty;
                    trendItem.TrendQuery = trend.query;
                    trendItem.TrendSearchURL = trend.url;
                    trendItem.Events = trend.events != null ? trend.events.ToString() : String.Empty;

                    trendTweets.Add(trendItem);
                }
            }

            return trendTweets;
        }

        protected ObservableCollection<Tweet> BuildTweetCollection(JToken tweetJsonToken)
        {
            ObservableCollection<Tweet> tweetResult = new ObservableCollection<Tweet>();
            JArray jsonToken = new JArray(tweetJsonToken);

            foreach (var jsonItem in jsonToken)
            {
                //var test = jsonItem.ToList<object>();
                foreach (var jitem in jsonItem)
                {
                    TweetHold tweetHold = jitem.ToObject<TweetHold>();
                    Tweet tweetItem = new Tweet();
                    tweetItem.CreateDate = tweetHold.created_at;
                    tweetItem.StatusID = tweetHold.id.ToString();
                    tweetItem.TweetText = tweetHold.text;
                    tweetItem.ScreenName = tweetHold.user.screen_name;
                    tweetItem.LocationName = tweetHold.place == null ? String.Empty : tweetHold.place.ToString();
                    tweetItem.UserID = tweetHold.user.id.ToString();
                    tweetItem.LocationCountry = tweetHold.geo == null ? String.Empty : tweetHold.geo.ToString();
                    tweetItem.TweetImage = new BitmapImage(new Uri(tweetHold.user.profile_image_url));
                    tweetItem.TweetUser = new User()
                    {
                        UserName = tweetHold.user.name,
                        TwitterID = tweetHold.user.id.ToString(),
                        ScreenName = tweetHold.user.screen_name,
                        CreateDate = tweetHold.user.created_at,
                        UserDescription = tweetHold.user.description,
                        FriendCount = tweetHold.user.friends_count,
                        FollowerCount = tweetHold.user.followers_count,
                        ProfileImage = new BitmapImage(new Uri(tweetHold.user.profile_image_url)),
                        Verified = tweetHold.user.verified,
                        ProtectedTweet = tweetHold.user._protected
                    };

                    tweetResult.Add(tweetItem);
                }

            }

            return tweetResult;

        }

        protected ObservableCollection<Tweet> BuildTweetCollection(JArray tweetJsonArray)
        {
            ObservableCollection<Tweet> tweetResult = new ObservableCollection<Tweet>();

            foreach (JToken jsonItem in tweetJsonArray)
            {
                TweetHold tweetHold = jsonItem.ToObject<TweetHold>();
                Tweet tweetItem = new Tweet();
                tweetItem.CreateDate = tweetHold.created_at;
                tweetItem.StatusID = tweetHold.id.ToString();
                tweetItem.TweetText = tweetHold.text;
                tweetItem.ScreenName = tweetHold.user.screen_name;
                tweetItem.LocationName = tweetHold.place == null ? String.Empty : tweetHold.place.ToString();
                tweetItem.UserID = tweetHold.user.id.ToString();
                tweetItem.LocationCountry = tweetHold.geo == null ? String.Empty : tweetHold.geo.ToString();
                tweetItem.TweetImage = new BitmapImage(new Uri(tweetHold.user.profile_image_url));
                tweetItem.TweetUser = new User()
                {
                    UserName = tweetHold.user.name,
                    TwitterID = tweetHold.user.id.ToString(),
                    ScreenName = tweetHold.user.screen_name,
                    CreateDate = tweetHold.user.created_at,
                    UserDescription = tweetHold.user.description,
                    FriendCount = tweetHold.user.friends_count,
                    FollowerCount = tweetHold.user.followers_count,
                    ProfileImage = new BitmapImage(new Uri(tweetHold.user.profile_image_url)),
                    Verified = tweetHold.user.verified,
                    ProtectedTweet = tweetHold.user._protected
                };

                tweetResult.Add(tweetItem);
            }

            return tweetResult;

        }


        public async Task<ObservableCollection<Tweet>> TweetsByUser(string UserScreenName)
        {
            string twitterRequestUrl, oAuthSignatureBaseString, oAuthHeaderString, twitterParamString, tweetResultString;
            Dictionary<string, string> twitterRequestParams;
            Dictionary<string, string> twitterOAuthParams;
            ObservableCollection<Tweet> userTweet = new ObservableCollection<Tweet>();
            //Task <ObservableCollection<Tweet>> tweetReturn;

            HttpClient httpTwitterClient = new HttpClient();
            HttpRequestMessage httpTweetRequest = new HttpRequestMessage();
            HttpResponseMessage httpTweetResponse;

            //Get Appropriate URL for Twitter Request
            twitterRequestUrl = GetTwitterAPIUrl(APIType.User);

            ////Get Appropriate Parameters for Twitter Request
            twitterRequestParams = GetTwitterAPIParams(APIType.User, UserScreenName);

            //Get OAuth Parameters
            twitterOAuthParams = GetAPIOAuthParams();

            //Create the OAuth Signature Base String with Twitter Request URL, Twitter Request Parameters, and OAuth Parameters (must be alphabetic order)
            oAuthSignatureBaseString = GetTwitterAPISignatureBase(twitterRequestUrl, twitterRequestParams, twitterOAuthParams);

            //Create OAuth Signature with OAuth Signature Base String and Appropriate OAuth Token Secret sent by SignatureAuth.Type 
            //Save to OAuth struct and add to OAuth Params dictionary
            OAuth.OAuthTypes.OAuthSignatureKey = _tweetAuthObj.OAuthSignature(oAuthSignatureBaseString, TweetOAuth.SignatureAuthType.AccessSecretToken);
            twitterOAuthParams[OAuth.OAuthTypes.OAuthSignatureString] = Uri.EscapeDataString(OAuth.OAuthTypes.OAuthSignatureKey);

            //Create OAuth Header String with OAuth Parameters
            oAuthHeaderString = twitterOAuthParams.Select(param => param.Key + "\"" + param.Value + "\"").Aggregate((p1, p2) => p1 + "," + p2);

            try
            {
                //Set HTTP Client object Request Headers and Content Buffer
                httpTwitterClient.DefaultRequestHeaders.ExpectContinue = false;
                httpTwitterClient.MaxResponseContentBufferSize = int.MaxValue;

                //Set HTTP Request Method
                httpTweetRequest.Method = HttpMethod.Get;

                //Set HTTP Request URI
                twitterParamString = twitterRequestParams.Select(param => param.Key + param.Value).Aggregate((p1, p2) => p1 + "&" + p2);
                httpTweetRequest.RequestUri = new Uri(twitterRequestUrl + "?" + twitterParamString);

                //OAuth Header
                httpTweetRequest.Headers.Authorization = new AuthenticationHeaderValue("OAuth", oAuthHeaderString);

                //Execute HTTP Client Request by SendAsync with HTTP Request Object
                httpTweetResponse = await httpTwitterClient.SendAsync(httpTweetRequest);

                //Set HTTP Response Object and evaluate
                if (httpTweetResponse.IsSuccessStatusCode)
                {
                    tweetResultString = await httpTweetResponse.Content.ReadAsStringAsync();

                    //Create Tweet Object and set properties from Twitter JSON string using JSON.NET
                    //Add each Tweet Object to ObservableCollection
                    //return to calling function
                    JArray tweetjArray = JArray.Parse(tweetResultString);
                    userTweet = BuildTweetCollection(tweetjArray);
                    //Debug.WriteLine("Raw tweet data:" + tweetResultString);
                    _tweetReturn = userTweet;

                }
                else
                {
                    return null;
                }

                return userTweet;
            }
            catch (Exception Err)
            {
                throw new Exception(Err.Message);
            }

        }


        public async Task<ObservableCollection<Tweet>> TweetsbySearch(string SearchQuery)
        {
            string twitterRequestUrl, oAuthSignatureBaseString, oAuthHeaderString, twitterParamString, tweetResultString;
            Dictionary<string, string> twitterRequestParams;
            Dictionary<string, string> twitterOAuthParams;
            ObservableCollection<Tweet> searchTweet = new ObservableCollection<Tweet>();

            HttpClient httpTwitterClient = new HttpClient();
            HttpRequestMessage httpTweetRequest = new HttpRequestMessage();
            HttpResponseMessage httpTweetResponse;

            //Get Appropriate URL for Twitter Request
            twitterRequestUrl = GetTwitterAPIUrl(APIType.Search);

            //Get Appropriate Parameters for Twitter Request
            twitterRequestParams = GetTwitterAPIParams(APIType.Search, SearchQuery);

            //Get OAuth Parameters
            twitterOAuthParams = GetAPIOAuthParams();

            //Create the OAuth Signature Base String with Twitter Request URL, Twitter Request Parameters, and OAuth Parameters (must be alphabetic order)
            oAuthSignatureBaseString = GetTwitterAPISignatureBase(twitterRequestUrl, twitterRequestParams, twitterOAuthParams);

            //Create OAuth Signature with OAuth Signature Base String and Appropriate OAuth Token Secret sent by SignatureAuth.Type 
            //Save to OAuth struct and add to OAuth Params dictionary
            OAuth.OAuthTypes.OAuthSignatureKey = _tweetAuthObj.OAuthSignature(oAuthSignatureBaseString, TweetOAuth.SignatureAuthType.AccessSecretToken);
            twitterOAuthParams[OAuth.OAuthTypes.OAuthSignatureString] = Uri.EscapeDataString(OAuth.OAuthTypes.OAuthSignatureKey);

            //Create OAuth Header String with OAuth Parameters
            oAuthHeaderString = twitterOAuthParams.Select(param => param.Key + "\"" + param.Value + "\"").Aggregate((p1, p2) => p1 + "," + p2);

            try
            {
                //Set HTTP Client object Request Headers and Content Buffer
                httpTwitterClient.DefaultRequestHeaders.ExpectContinue = false;
                httpTwitterClient.MaxResponseContentBufferSize = int.MaxValue;

                //Set HTTP Request Method
                httpTweetRequest.Method = HttpMethod.Get;

                //Set HTTP Request URI
                twitterParamString = twitterRequestParams.Select(param => param.Key + param.Value).Aggregate((p1, p2) => p1 + "&" + p2);
                httpTweetRequest.RequestUri = new Uri(twitterRequestUrl + "?" + twitterParamString);

                //OAuth Header
                httpTweetRequest.Headers.Authorization = new AuthenticationHeaderValue("OAuth", oAuthHeaderString);

                //Execute HTTP Client Request by SendAsync with HTTP Request Object
                httpTweetResponse = await httpTwitterClient.SendAsync(httpTweetRequest);

                //Set HTTP Response Object and evaluate
                if (httpTweetResponse.IsSuccessStatusCode)
                {
                    tweetResultString = await httpTweetResponse.Content.ReadAsStringAsync();

                    JObject result = JObject.Parse(tweetResultString);
                    JToken jResult = result["statuses"];
                    searchTweet = BuildTweetCollection(jResult);
                    _tweetReturn = searchTweet;
                    //TweetHold test = result.ToObject<TweetHold>();
                    //JsonConvert.DeserializeObject<TweetHold>(tweetResultString);

                }
                else
                {
                    return null;
                }

                return searchTweet;
            }
            catch (Exception Err)
            {
                throw new Exception(Err.Message);
            }

        }

        public async Task<ObservableCollection<Trend>> TweetsbyTrend(string TrendLocation)
        {
            string twitterRequestUrl, oAuthSignatureBaseString, oAuthHeaderString, twitterParamString, tweetResultString;
            Dictionary<string, string> twitterRequestParams;
            Dictionary<string, string> twitterOAuthParams;
            ObservableCollection<Trend> trendTweet = new ObservableCollection<Trend>();
            //Task <ObservableCollection<Tweet>> tweetReturn;

            HttpClient httpTwitterClient = new HttpClient();
            HttpRequestMessage httpTweetRequest = new HttpRequestMessage();
            HttpResponseMessage httpTweetResponse;

            //Get Appropriate URL for Twitter Request
            twitterRequestUrl = GetTwitterAPIUrl(APIType.Trend);

            //Get Appropriate Parameters for Twitter Request
            twitterRequestParams = GetTwitterAPIParams(APIType.Trend, TrendLocation);

            //Get OAuth Parameters
            twitterOAuthParams = GetAPIOAuthParams();

            //Create the OAuth Signature Base String with Twitter Request URL, Twitter Request Parameters, and OAuth Parameters (must be alphabetic order)
            oAuthSignatureBaseString = GetTwitterAPISignatureBase(twitterRequestUrl, twitterRequestParams, twitterOAuthParams);

            //Create OAuth Signature with OAuth Signature Base String and Appropriate OAuth Token Secret sent by SignatureAuth.Type 
            //Save to OAuth struct and add to OAuth Params dictionary
            OAuth.OAuthTypes.OAuthSignatureKey = _tweetAuthObj.OAuthSignature(oAuthSignatureBaseString, TweetOAuth.SignatureAuthType.AccessSecretToken);
            twitterOAuthParams[OAuth.OAuthTypes.OAuthSignatureString] = Uri.EscapeDataString(OAuth.OAuthTypes.OAuthSignatureKey);

            //Create OAuth Header String with OAuth Parameters
            oAuthHeaderString = twitterOAuthParams.Select(param => param.Key + "\"" + param.Value + "\"").Aggregate((p1, p2) => p1 + "," + p2);

            try
            {
                //Set HTTP Client object Request Headers and Content Buffer
                httpTwitterClient.DefaultRequestHeaders.ExpectContinue = false;
                httpTwitterClient.MaxResponseContentBufferSize = int.MaxValue;

                //Set HTTP Request Method
                httpTweetRequest.Method = HttpMethod.Get;

                //Set HTTP Request URI
                twitterParamString = twitterRequestParams.Select(param => param.Key + param.Value).Aggregate((p1, p2) => p1 + "&" + p2);
                httpTweetRequest.RequestUri = new Uri(twitterRequestUrl + "?" + twitterParamString);

                //OAuth Header
                httpTweetRequest.Headers.Authorization = new AuthenticationHeaderValue("OAuth", oAuthHeaderString);

                //Execute HTTP Client Request by SendAsync with HTTP Request Object
                httpTweetResponse = await httpTwitterClient.SendAsync(httpTweetRequest);

                //Set HTTP Response Object and evaluate
                if (httpTweetResponse.IsSuccessStatusCode)
                {
                    tweetResultString = await httpTweetResponse.Content.ReadAsStringAsync();

                    //Create Tweet Object and set properties from Twitter JSON string using JSON.NET
                    //Add each Tweet Object to ObservableCollection
                    //return to calling function
                    JArray tweetjArray = JArray.Parse(tweetResultString);
                    trendTweet = BuildTrendCollection(tweetjArray);
                    //Debug.WriteLine("Raw tweet data:" + tweetResultString);
                    //_tweetReturn = trendTweet;

                }
                else
                {
                    return null;
                }

                return trendTweet;
            }
            catch (Exception Err)
            {
                throw new Exception(Err.Message);
            }

            //return _tweetReturn;
        }

        public async Task<ObservableCollection<Tweet>> TweetsbyHomeTimeline()
        {
            string twitterRequestUrl, oAuthSignatureBaseString, oAuthHeaderString, twitterParamString, tweetResultString;
            Dictionary<string, string> twitterRequestParams;
            Dictionary<string, string> twitterOAuthParams;
            ObservableCollection<Tweet> homeTweet = new ObservableCollection<Tweet>();
            //Task <ObservableCollection<Tweet>> tweetReturn;

            HttpClient httpTwitterClient = new HttpClient();
            HttpRequestMessage httpTweetRequest = new HttpRequestMessage();
            HttpResponseMessage httpTweetResponse;

            //Get Appropriate URL for Twitter Request
            twitterRequestUrl = GetTwitterAPIUrl(APIType.Home);

            //Get Appropriate Parameters for Twitter Request
            twitterRequestParams = GetTwitterAPIParams(APIType.Home, null);

            //Get OAuth Parameters
            twitterOAuthParams = GetAPIOAuthParams();

            //Create the OAuth Signature Base String with Twitter Request URL, Twitter Request Parameters, and OAuth Parameters (must be alphabetic order)
            oAuthSignatureBaseString = GetTwitterAPISignatureBase(twitterRequestUrl, twitterRequestParams, twitterOAuthParams);

            //Create OAuth Signature with OAuth Signature Base String and Appropriate OAuth Token Secret sent by SignatureAuth.Type 
            //Save to OAuth struct and add to OAuth Params dictionary
            OAuth.OAuthTypes.OAuthSignatureKey = _tweetAuthObj.OAuthSignature(oAuthSignatureBaseString, TweetOAuth.SignatureAuthType.AccessSecretToken);
            twitterOAuthParams[OAuth.OAuthTypes.OAuthSignatureString] = Uri.EscapeDataString(OAuth.OAuthTypes.OAuthSignatureKey);

            //Create OAuth Header String with OAuth Parameters
            oAuthHeaderString = twitterOAuthParams.Select(param => param.Key + "\"" + param.Value + "\"").Aggregate((p1, p2) => p1 + "," + p2);

            try
            {
                //Set HTTP Client object Request Headers and Content Buffer
                httpTwitterClient.DefaultRequestHeaders.ExpectContinue = false;
                httpTwitterClient.MaxResponseContentBufferSize = int.MaxValue;

                //Set HTTP Request Method
                httpTweetRequest.Method = HttpMethod.Get;

                //Set HTTP Request URI
                twitterParamString = twitterRequestParams.Select(param => param.Key + param.Value).Aggregate((p1, p2) => p1 + "&" + p2);
                httpTweetRequest.RequestUri = new Uri(twitterRequestUrl + "?" + twitterParamString);

                //OAuth Header
                httpTweetRequest.Headers.Authorization = new AuthenticationHeaderValue("OAuth", oAuthHeaderString);

                //Execute HTTP Client Request by SendAsync with HTTP Request Object
                httpTweetResponse = await httpTwitterClient.SendAsync(httpTweetRequest);

                //Set HTTP Response Object and evaluate
                if (httpTweetResponse.IsSuccessStatusCode)
                {
                    tweetResultString = await httpTweetResponse.Content.ReadAsStringAsync();

                    //Create Tweet Object and set properties from Twitter JSON string using JSON.NET
                    //Add each Tweet Object to ObservableCollection
                    //return to calling function
                    JArray tweetjArray = JArray.Parse(tweetResultString);
                    homeTweet = BuildTweetCollection(tweetjArray);
                    //Debug.WriteLine("Raw tweet data:" + tweetResultString);
                    _tweetReturn = homeTweet;

                }
                else
                {
                    return null;
                }

                return homeTweet;
            }
            catch (Exception Err)
            {
                throw new Exception(Err.Message);
            }

        }

        public async Task<ObservableCollection<User>> FollowersByUser(string UserName)
        {
            string twitterRequestUrl, oAuthSignatureBaseString, oAuthHeaderString, twitterParamString, userResultString;
            Dictionary<string, string> twitterRequestParams;
            Dictionary<string, string> twitterOAuthParams;
            ObservableCollection<User> userFollowers = new ObservableCollection<User>();

            HttpClient httpTwitterClient = new HttpClient();
            HttpRequestMessage httpTweetRequest = new HttpRequestMessage();
            HttpResponseMessage httpTweetResponse;

            //Get Appropriate URL for Twitter Request
            twitterRequestUrl = GetTwitterAPIUrl(APIType.Follower);

            //Get Appropriate Parameters for Twitter Request
            twitterRequestParams = GetTwitterAPIParams(APIType.Follower, UserName);

            //Get OAuth Parameters
            twitterOAuthParams = GetAPIOAuthParams();

            //Create the OAuth Signature Base String with Twitter Request URL, Twitter Request Parameters, and OAuth Parameters (must be alphabetic order)
            oAuthSignatureBaseString = GetTwitterAPISignatureBase(twitterRequestUrl, twitterRequestParams, twitterOAuthParams);

            //Create OAuth Signature with OAuth Signature Base String and Appropriate OAuth Token Secret sent by SignatureAuth.Type 
            //Save to OAuth struct and add to OAuth Params dictionary
            OAuth.OAuthTypes.OAuthSignatureKey = _tweetAuthObj.OAuthSignature(oAuthSignatureBaseString, TweetOAuth.SignatureAuthType.AccessSecretToken);
            twitterOAuthParams[OAuth.OAuthTypes.OAuthSignatureString] = Uri.EscapeDataString(OAuth.OAuthTypes.OAuthSignatureKey);

            //Create OAuth Header String with OAuth Parameters
            oAuthHeaderString = twitterOAuthParams.Select(param => param.Key + "\"" + param.Value + "\"").Aggregate((p1, p2) => p1 + "," + p2);

            try
            {
                //Set HTTP Client object Request Headers and Content Buffer
                httpTwitterClient.DefaultRequestHeaders.ExpectContinue = false;
                httpTwitterClient.MaxResponseContentBufferSize = int.MaxValue;

                //Set HTTP Request Method
                httpTweetRequest.Method = HttpMethod.Get;

                //Set HTTP Request URI
                twitterParamString = twitterRequestParams.Select(param => param.Key + param.Value).Aggregate((p1, p2) => p1 + "&" + p2);
                httpTweetRequest.RequestUri = new Uri(twitterRequestUrl + "?" + twitterParamString);

                //OAuth Header
                httpTweetRequest.Headers.Authorization = new AuthenticationHeaderValue("OAuth", oAuthHeaderString);

                //Execute HTTP Client Request by SendAsync with HTTP Request Object
                httpTweetResponse = await httpTwitterClient.SendAsync(httpTweetRequest);

                //Set HTTP Response Object and evaluate
                if (httpTweetResponse.IsSuccessStatusCode)
                {
                    userResultString = await httpTweetResponse.Content.ReadAsStringAsync();

                    //Create User Object and set properties from Twitter JSON string using JSON.NET
                    //Add each Tweet Object to ObservableCollection
                    //return to calling function
                    JObject result = JObject.Parse(userResultString);
                    JToken jTokenResult = result["users"];
                    userFollowers = BuildUserCollection(jTokenResult);
                    _usersReturn = userFollowers;

                }
                else
                {
                    return null;
                }

            }
            catch (Exception Err)
            {
                throw new Exception(Err.Message);
            }
            
            return _usersReturn;
        }

        public async Task<User> TwitterUserInfo(string CurrentUser)
        {
            return _twitterUserInfo;
        
        }

        protected ObservableCollection<User> BuildUserCollection (JToken userJsonToken)
        { 
            ObservableCollection<User> userResult = new ObservableCollection<User>();
            JArray jsonToken = new JArray(userJsonToken);

            foreach (var jsonItem in jsonToken)
            {
                foreach (var jItem in jsonItem)
                {
                    TweetUser userHold = jItem.ToObject<TweetUser>();
                    User userItem = new User();
                    userItem.UserName = userHold.name.ToString();
                    userItem.TwitterID = userHold.id_str;
                    userItem.ScreenName = userHold.screen_name;
                    userItem.CreateDate = userHold.created_at;
                    userItem.UserDescription = userHold.description;
                    userItem.FriendCount = userHold.friends_count;
                    userItem.FollowerCount = userHold.followers_count;
                    userItem.ProfileImage = new BitmapImage(new Uri(userHold.profile_image_url));
                    userItem.Verified = userHold.verified;
                    userItem.ProtectedTweet = userHold._protected;

                    userResult.Add(userItem);
                }
            }

            return userResult;

        }

        protected ObservableCollection<User> BuildUserCollection(JArray userJsonArray) 
        {
            ObservableCollection<User> userResult = new ObservableCollection<User>();

            foreach (JToken jsonItem in userJsonArray)
            {
                TweetUser userHold = jsonItem.ToObject<TweetUser>();
                User userItem = new User();
                userItem.UserName = userHold.name.ToString();
                userItem.TwitterID = userHold.id_str;
                userItem.ScreenName = userHold.screen_name;
                userItem.CreateDate = userHold.created_at;
                userItem.UserDescription = userHold.description;
                userItem.FriendCount = userHold.friends_count;
                userItem.FollowerCount = userHold.followers_count;
                userItem.ProfileImage = new BitmapImage(new Uri(userHold.profile_image_url));
                userItem.Verified = userHold.verified;
                userItem.ProtectedTweet = userHold._protected;                

                userResult.Add(userItem);
            }
            
            return userResult;
        }

    }


    
}
