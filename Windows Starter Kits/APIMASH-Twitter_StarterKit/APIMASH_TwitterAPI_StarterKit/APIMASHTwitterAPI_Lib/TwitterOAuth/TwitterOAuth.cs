using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TwitterAPIWin8Lib.APICommon;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;


///<summary>
///Required OAuth calls for the Twitter API library for Twitter v1.1 which requires all calls
///to adhere to strict OAuth requirements for EACH Twitter API call
///DO NOT Alter or Change this Authentication Library in order to ensure correct Auth calls
///</summary>

namespace TwitterAPIWin8Lib.TwitterOAuth
{
    public enum TweetAuthType
    {
        AppAuth = 1,
        OAuth = 2
    }

    public struct TweetAuthConstants
    {
        public const string BearerToken = "https://api.twitter.com/oauth2/token";
        public const string RequestToken = "https://api.twitter.com/oauth/request_token";
        public const string AuthorizeURL = "https://api.twitter.com/oauth/authorize";
        public const string AccessToken = "https://api.twitter.com/oauth/access_token";
        public static string TweetSignatureMethod = "HMAC-SHA1";
        public static string NoCallBack = "oob";
        public static string OAuthUserID = String.Empty;
        public static string OAuthUserScreenName = String.Empty;
    }

    public static class OAuth
    {
        public static ApplicationDataContainer OAuthCredentialStorage;

        public struct OAuthTypes
        {
            public const string OAuthAccessTypeString = "oauth_access_type=";
            public const string OAuthConsumerKeyString = "oauth_consumer_key=";
            public const string OAuthCallbackString = "oauth_callback=";
            public const string OAuthVersionString = "oauth_version=";
            public const string OAuthXAccessTypeString = "x_auth_access_type=";
            public const string OAuthSignatureMethodString = "oauth_signature_method=";
            public const string OAuthSignatureString = "oauth_signature=";
            public const string OAuthTimestampString = "oauth_timestamp=";
            public const string OAuthNonceString = "oauth_nonce=";
            public const string OAuthTokenString = "oauth_token=";
            public const string OAuthVerifierString = "oauth_verifier=";
            public const string OAuthTokenSecretString = "oauth_token_secret=";
            public static string OAuthConsumerSecret = String.Empty;
            public static string OAuthAccessTypeKey = String.Empty;
            public static string OAuthConsumerKey = String.Empty;
            public static string OAuthCallbackKey = "http://blogs.msdn.com/tarawalker"; //String.Empty;
            public static string OAuthVersionKey = "1.0"; //String.Empty;
            public static string OAuthXAccessTypeKey = String.Empty;
            public static string OAuthSignatureMethodKey = String.Empty;
            public static string OAuthSignatureKey = String.Empty;
            public static string OAuthTimestampKey = String.Empty;
            public static string OAuthNonceKey = String.Empty;
            public static string OAuthRequestTokenKey = String.Empty;
            public static string OAuthRequestTokenSecretKey = String.Empty;
            public static string OAuthAccessTokenKey = String.Empty;
            public static string OAuthVerifierKey = String.Empty;
            public static string OAuthAccessTokenSecretKey = String.Empty;
        }

        //TODO: If this is passed in via the Constructor, make scope protected
       
    }

    public static class AppAuth
    {
        public struct AppAuthTypes
        {
            public const string AppAuthConsumerSecretString = "oauth_access_type=";
            public const string AppAuthConsumerKeyString = "oauth_consumer_key=";
            public static string AppAuthConsumerSecret = String.Empty;
            public static string AppAuthConsumerKey = String.Empty;
            public static string AppAuthBearerTokenKey = String.Empty;
            public static string AppAuthBearerTokenString = String.Empty;
            public static string AppAuthBasicToken = String.Empty;
            public static string AppAuthTokenCredentials = String.Empty;
        }

    }

    public class TweetOAuth
    {
        public enum SignatureAuthType
        {
            ConsumerSecret,
            RequestSecretToken,
            AccessSecretToken
        }

        private bool _authenticatedUser = false;
        public bool AuthenticatedUser
        {
            get { return _authenticatedUser; }
            set
            { if (_authenticatedUser != value) { _authenticatedUser = value; } }
        }


        //Constructor Overloaded 
        //TODO: Change Constructors to get current App Data Storage

        //Constructor if App and User Already Authenticated
        public TweetOAuth()
        {
            //TODO: Change this
            if (String.IsNullOrEmpty(OAuth.OAuthTypes.OAuthAccessTokenKey))
            {
                throw new UnauthorizedAccessException("User Not Authenticated, Please Instantiate Object with appropriate Constructor");
            }
            else { AuthenticatedUser = true; }
        }

        public TweetOAuth(string ConsumerKey, string ConsumerSecret, TweetAuthType SelectedAuth)
        {
            OAuth.OAuthTypes.OAuthConsumerKey = ConsumerKey;
            OAuth.OAuthTypes.OAuthConsumerSecret = ConsumerSecret;
            AppAuth.AppAuthTypes.AppAuthConsumerKey = ConsumerKey;
            AppAuth.AppAuthTypes.AppAuthConsumerSecret = ConsumerSecret;
            AuthenticationType = SelectedAuth;
            

        }

        public async Task AuthenticateTwitterByOAuth()
        {
            //Debug.WriteLine("In AuthenicateTwitterByOAuth Method");
            string OAuthRequest, OAuthResponse, twitterParams, twitterSignature;

            twitterParams = BuildOAuthParams(true);
            //Debug.WriteLine(twitterParams);

            String signBase = HttpMethod.Get.ToString().ToUpper() + "&";
            signBase += Uri.EscapeDataString(TweetAuthConstants.RequestToken) + "&" + Uri.EscapeDataString(twitterParams);
            //Debug.WriteLine(signBase);

            //Build and Get Twitter Signature
            twitterSignature = OAuthSignature(signBase, SignatureAuthType.ConsumerSecret);
            //Debug.WriteLine(twitterSignature);

            //Build URL to Request Token
            OAuthRequest = TweetAuthConstants.RequestToken + "?" + twitterParams + "&" + OAuth.OAuthTypes.OAuthSignatureString + Uri.EscapeDataString(twitterSignature);
            //Debug.WriteLine(OAuthRequest);

            //Obtain Request Token 
            OAuthResponse = await OAuthRequestToken(OAuthRequest);
            //Debug.WriteLine("Received Data: " + OAuthResponse);

            //Get Access Token
            GetOAuthTokenKeys(OAuthResponse);

            //Authorize Access Token
            await AuthorizeOAuthTokens();

            //Get the Authorize Access Token
            await GetOAuthAccessTokens();

            //Once Authorized so User won't have to repeat
            StoreOAuthUserInfo();

            this.AuthenticatedUser = true;

            //AuthorizeOAuthTokens();
            //return this.AuthenticatedUser;
        }

        public async Task AuthenticateTwitterbyAppAuth()
        {
            AppAuth.AppAuthTypes.AppAuthBasicToken = AppAuthTokenEncoding();
            await AuthorizeAppAuthTokens();
            //return this.AuthenticatedUser;
        }

        public async Task AuthorizeAppAuthTokens()
        {
            HttpClient httpPostClient;
            HttpRequestMessage httpRequestMsg;
            HttpResponseMessage httpResponseMsg;

            string authData, authResponse;
            authData = "grant_type=client_credentials";

            httpPostClient = new HttpClient();
            httpPostClient.MaxResponseContentBufferSize = int.MaxValue;
            //httpClient.DefaultRequestHeaders.ExpectContinue = false;

            httpRequestMsg = new HttpRequestMessage();
            httpRequestMsg.Content = new StringContent(authData);
            httpRequestMsg.Method = new HttpMethod("POST");
            httpRequestMsg.RequestUri = new Uri(TweetAuthConstants.BearerToken, UriKind.Absolute);
            httpRequestMsg.Headers.Authorization = new AuthenticationHeaderValue("Basic", AppAuth.AppAuthTypes.AppAuthBasicToken);
            httpRequestMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");


            httpResponseMsg = await httpPostClient.SendAsync(httpRequestMsg);
            //return await response.Content.ReadAsStringAsync();

            if (httpResponseMsg.IsSuccessStatusCode)
            {
                //Debug.WriteLine("Http Response returned Successful");
                authResponse = await httpResponseMsg.Content.ReadAsStringAsync();
                //Debug.WriteLine(authResponse);


                if (authResponse == null || String.IsNullOrEmpty(authResponse))
                {
                    this.AuthenticatedUser = false;
                    throw new MissingMemberException("Authenication Response Returned but With No Value");

                }

                this.AuthenticatedUser = true;
                var jsonResponse = authResponse.Split(',');

                var authTokens = jsonResponse.Select((item, index) => item.Split(':'));
                foreach (var itemData in authTokens)
                {
                    string tokenDataType = itemData[0].Replace('{', ' ').Trim().Trim('\"');

                    string tokenKey = itemData[1].Trim('\"').Trim('}', ' ');

                    if (tokenDataType == "access_token")
                    {
                        //Debug.WriteLine("Got Access Token");
                        //Debug.WriteLine(tokenDataType);
                        //Debug.WriteLine(tokenKey);

                        AppAuth.AppAuthTypes.AppAuthBearerTokenKey = tokenKey;
                    }
                    else { AppAuth.AppAuthTypes.AppAuthBearerTokenString = tokenDataType; }
                }

            }
            else { this.AuthenticatedUser = false; }

            //return this.AuthenticatedUser;
        }

        public TweetAuthType AuthenticationType { get; set; }

        public string OAuthSignature(string SigBaseString, SignatureAuthType signType)
        {
            string signingKey = string.Empty;
            //Debug.WriteLine("In OAuthSignature function");

            switch (signType)
            {
                case SignatureAuthType.ConsumerSecret:
                    signingKey = string.Format("{0}&{1}", OAuth.OAuthTypes.OAuthConsumerSecret, "");
                    break;
                case SignatureAuthType.RequestSecretToken:
                    signingKey = string.Format("{0}&{1}", OAuth.OAuthTypes.OAuthConsumerSecret, !string.IsNullOrEmpty(OAuth.OAuthTypes.OAuthRequestTokenSecretKey) ? OAuth.OAuthTypes.OAuthRequestTokenSecretKey : String.Empty);
                    break;
                case SignatureAuthType.AccessSecretToken:
                    signingKey = string.Format("{0}&{1}", OAuth.OAuthTypes.OAuthConsumerSecret, !string.IsNullOrEmpty(OAuth.OAuthTypes.OAuthAccessTokenSecretKey) ? OAuth.OAuthTypes.OAuthAccessTokenSecretKey : String.Empty);
                    break;
                default:
                    break;
            }

            IBuffer KeyBuffer = CryptographicBuffer.ConvertStringToBinary(signingKey, BinaryStringEncoding.Utf8);
            MacAlgorithmProvider HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);
            CryptographicKey MacKey = HmacSha1Provider.CreateKey(KeyBuffer);
            IBuffer DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(SigBaseString, BinaryStringEncoding.Utf8);
            IBuffer SignatureBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);
            String Signature = CryptographicBuffer.EncodeToBase64String(SignatureBuffer);
            //Debug.WriteLine("Signed Signature: " + Signature);
            return Signature;

        }

        public async Task<string> OAuthRequestToken(string requestUrl)
        {
            //Debug.WriteLine("In OAuthRequestToken function");

            HttpClient webClient = new HttpClient();
            string webResponse = String.Empty;

            //Debug.WriteLine("Request Url: " + requestUrl);

            try { return await webClient.GetStringAsync(requestUrl); }
            catch (Exception Err)
            {
                //Debug.WriteLine(Err);
                webResponse = Err.ToString();
            }

            return null;

        }

        public string BuildOAuthParams(bool _doCallback)
        {

            string OAuthParameters = String.Empty;
            string Nonce = OAuthNonce();
            string timeStamp = OAuthTimeStamp(HttpMethod.Get);

            OAuth.OAuthTypes.OAuthSignatureMethodKey = TweetAuthConstants.TweetSignatureMethod;
            if (_doCallback)
            {
                OAuthParameters += OAuth.OAuthTypes.OAuthCallbackString +
                    Uri.EscapeDataString(OAuth.OAuthTypes.OAuthCallbackKey == String.Empty ? TweetAuthConstants.NoCallBack : OAuth.OAuthTypes.OAuthCallbackKey) + "&";
            }
            OAuthParameters += OAuth.OAuthTypes.OAuthConsumerKeyString + OAuth.OAuthTypes.OAuthConsumerKey + "&";
            OAuthParameters += OAuth.OAuthTypes.OAuthNonceString + Nonce + "&";
            OAuthParameters += OAuth.OAuthTypes.OAuthSignatureMethodString + OAuth.OAuthTypes.OAuthSignatureMethodKey + "&";
            OAuthParameters += OAuth.OAuthTypes.OAuthTimestampString + timeStamp + "&";
            OAuthParameters += OAuth.OAuthTypes.OAuthVersionString + OAuth.OAuthTypes.OAuthVersionKey;


            //Debug.WriteLine("Nonce: " + Nonce);
            //Debug.WriteLine("TimeStamp; " + timeStamp);
            //Debug.WriteLine("Parameters: " + OAuthParameters);

            return OAuthParameters;
        }



        public string OAuthNonce()
        {
            Random randNum = new Random();
            Int32 nonce = randNum.Next(1000000000);
            return nonce.ToString();

        }

        public string OAuthTimeStamp(HttpMethod httpMethod)
        {
            string TimeStamp = String.Empty;

            if (httpMethod == HttpMethod.Get)
            {
                TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
                //Debug.WriteLine("Rounded SinceEpoch: " + Math.Round(SinceEpoch.TotalSeconds));
                TimeStamp = Math.Round(SinceEpoch.TotalSeconds).ToString();
            }
            else
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeStamp = Convert.ToInt64(ts.TotalSeconds).ToString();
            }

            return TimeStamp;
        }

        private async Task<String> SendDataAsync(String Url)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                return await httpClient.GetStringAsync(Url);
            }
            catch (Exception Err)
            {
                //Debug.WriteLine(Err.ToString());

            }

            return null;
        }

        public void GetOAuthTokenKeys(string OAuthResponse)
        {
            //Debug.WriteLine("In GetOAuthTokenKeys function");
            //String[] keyValPairs, splits;
            string OAuthToken = String.Empty, OAuthTokenSecret = String.Empty;

            //Cleaning up my code, only need 3 lines to replace 13 lines of code below
            Dictionary<string, string> oauthResult = OAuthResponse.Split('&').Select(param => param.Split('=')).ToDictionary(item => item[0], item => item[1]);
            OAuth.OAuthTypes.OAuthRequestTokenKey = oauthResult["oauth_token"];
            OAuth.OAuthTypes.OAuthRequestTokenSecretKey = oauthResult["oauth_token_secret"];

            //Debug.WriteLine("Request Tokens Acquired, and AuthenticatedUser is " + this.AuthenticatedUser);

        }

        protected async Task AuthorizeOAuthTokens()
        {
            //Debug.WriteLine("In AuthorizeOAuthToken function");

            HttpClient tweetClient = new HttpClient();
            string authorizeRequestUrl, authorizeResponse;
            Uri requestUri, callbackUri;
            //HttpResponseMessage authResponse;
            //HttpRequestMessage authRequest  //"https://api.twitter.com/oauth/authorize?oauth_token=" + oauth_token;

            authorizeRequestUrl = TweetAuthConstants.AuthorizeURL + "?" + OAuth.OAuthTypes.OAuthTokenString + OAuth.OAuthTypes.OAuthRequestTokenKey;
            requestUri = new Uri(authorizeRequestUrl, UriKind.Absolute);
            callbackUri = new Uri(OAuth.OAuthTypes.OAuthCallbackKey);

            //Debug.WriteLine("Navigating to: " + authorizeRequestUrl); // //DebugPrint("Navigating to: " + TwitterUrl);

            WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                                WebAuthenticationOptions.None,
                                                                requestUri,
                                                                callbackUri);

            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                //Debug.WriteLine("Access Tokens Acquired, User Authorized and AuthenicatedUser is " + this.AuthenticatedUser);

                authorizeResponse = webAuthenticationResult.ResponseData.ToString();
                Dictionary<string, string> authData = authorizeResponse.Split('?').ToList()[1].Split('&').Select(p => p.Split('=')).ToDictionary(param => param[0], param => param[1]);
                OAuth.OAuthTypes.OAuthAccessTokenKey = authData["oauth_token"];
                OAuth.OAuthTypes.OAuthVerifierKey = authData["oauth_verifier"];
            }
            else
            {
                this.AuthenticatedUser = false; //Debug.WriteLine("User Not Authorized, In Tweet OAuth and AuthenicatedUser is " + this.AuthenticatedUser);
                throw new Exception(webAuthenticationResult.ResponseErrorDetail.ToString());
            }

        }

        protected async Task GetOAuthAccessTokens()
        {
            string tokenResponse;

            OAuth.OAuthTypes.OAuthNonceKey = OAuthNonce();
            OAuth.OAuthTypes.OAuthTimestampKey = OAuthTimeStamp(HttpMethod.Get);
            OAuth.OAuthTypes.OAuthSignatureMethodKey = TweetAuthConstants.TweetSignatureMethod;

            //string testVerifier = String.IsNullOrEmpty(oAuthVerifier.Text) ? String.Empty : oAuthVerifier.Text;

            string sigBaseStringParams = OAuth.OAuthTypes.OAuthConsumerKeyString + OAuth.OAuthTypes.OAuthConsumerKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthNonceString + OAuth.OAuthTypes.OAuthNonceKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthSignatureMethodString + OAuth.OAuthTypes.OAuthSignatureMethodKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthTimestampString + OAuth.OAuthTypes.OAuthTimestampKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthTokenString + OAuth.OAuthTypes.OAuthRequestTokenKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthVerifierString + OAuth.OAuthTypes.OAuthVerifierKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthVersionString + OAuth.OAuthTypes.OAuthVersionKey;

            string sigBaseString = "POST&";
            sigBaseString += Uri.EscapeDataString(TweetAuthConstants.AccessToken) + "&" + Uri.EscapeDataString(sigBaseStringParams);

            OAuth.OAuthTypes.OAuthSignatureKey = OAuthSignature(sigBaseString, SignatureAuthType.RequestSecretToken);
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthSignatureString + Uri.EscapeDataString(OAuth.OAuthTypes.OAuthSignatureKey);

            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.MaxResponseContentBufferSize = int.MaxValue;
                httpClient.DefaultRequestHeaders.ExpectContinue = false;
                HttpRequestMessage requestMsg = new HttpRequestMessage();
                requestMsg.Content = new StringContent(sigBaseStringParams);
                requestMsg.Method = new HttpMethod("POST");
                requestMsg.RequestUri = new Uri(TweetAuthConstants.AccessToken, UriKind.Absolute);
                requestMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var response = await httpClient.SendAsync(requestMsg);
                tokenResponse = await response.Content.ReadAsStringAsync();

            }
            catch (Exception Err)
            {
                throw new Exception(Err.Message);
            }

            if (!string.IsNullOrEmpty(tokenResponse))
            {
                Dictionary<string, string> oauthValPairs = tokenResponse.Split('&').Select(item => item.Split('=')).ToDictionary(p => p[0], p => p[1]);

                OAuth.OAuthTypes.OAuthAccessTokenKey = oauthValPairs["oauth_token"];
                OAuth.OAuthTypes.OAuthAccessTokenSecretKey = oauthValPairs["oauth_token_secret"];
                TweetAuthConstants.OAuthUserID = oauthValPairs["user_id"];
                TweetAuthConstants.OAuthUserScreenName = oauthValPairs["screen_name"];
            }

        }

        public void StoreOAuthUserInfo()
        {
            //TODO: Change this to have ApplicationDataContainer that should have been passed in.

            //ApplicationData.Current.LocalSettings.DeleteContainer("TwitterAPIMash");
            ApplicationDataContainer appLocalData = ApplicationData.Current.LocalSettings.CreateContainer(CommonProperties.TwitterAPI_DataStorage, ApplicationDataCreateDisposition.Always);

            //Save or Update OAuth values in appLocalData if applicable, so won't have to Re-Authorize
            if (appLocalData.Values.ContainsKey("OAuthConsumerKey")) { appLocalData.Values["OAuthConsumerKey"] = OAuth.OAuthTypes.OAuthConsumerKey; }
            else { appLocalData.Values.Add("OAuthConsumerKey", OAuth.OAuthTypes.OAuthConsumerKey); }

            if (appLocalData.Values.ContainsKey("OAuthConsumerSecret")) { appLocalData.Values["OAuthConsumerSecret"] = OAuth.OAuthTypes.OAuthConsumerSecret; }
            else { appLocalData.Values.Add("OAuthConsumerSecret", OAuth.OAuthTypes.OAuthConsumerSecret); }

            if (appLocalData.Values.ContainsKey("OAuthAccessToken")) { appLocalData.Values["OAuthAccessToken"] = OAuth.OAuthTypes.OAuthAccessTokenKey; }
            else { appLocalData.Values.Add("OAuthAccessToken", OAuth.OAuthTypes.OAuthAccessTokenKey); }

            if (appLocalData.Values.ContainsKey("OAuthAccessSecret")) { appLocalData.Values["OAuthAccessSecret"] = OAuth.OAuthTypes.OAuthAccessTokenSecretKey; }
            else { appLocalData.Values.Add("OAuthAccessSecret", OAuth.OAuthTypes.OAuthAccessTokenSecretKey); }

            if (appLocalData.Values.ContainsKey("OAuthVerifier")) { appLocalData.Values["OAuthVerifier"] = OAuth.OAuthTypes.OAuthVerifierKey; }
            else { appLocalData.Values.Add("OAuthVerifier", OAuth.OAuthTypes.OAuthVerifierKey); }

            if (appLocalData.Values.ContainsKey("UserID")) { appLocalData.Values["UserID"] = TweetAuthConstants.OAuthUserID; }
            else { appLocalData.Values.Add("UserID", TweetAuthConstants.OAuthUserID); }

            if (appLocalData.Values.ContainsKey("UserScreenName")) { appLocalData.Values["UserScreenName"] = TweetAuthConstants.OAuthUserScreenName; }
            else { appLocalData.Values.Add("UserScreenName", TweetAuthConstants.OAuthUserScreenName); }

        }

        public string AppAuthTokenEncoding()
        {
            string encodedConsumerKey, encodedConsumerSecretKey, encodedAuthTokenKey;

            encodedConsumerKey = WebUtility.UrlEncode(AppAuth.AppAuthTypes.AppAuthConsumerKey);
            encodedConsumerSecretKey = WebUtility.UrlEncode(AppAuth.AppAuthTypes.AppAuthConsumerSecret);

            encodedAuthTokenKey = String.Concat(encodedConsumerKey, ":", encodedConsumerSecretKey);


            return Convert.ToBase64String(Encoding.UTF8.GetBytes(encodedAuthTokenKey));

        }

        
    }
}
