using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMPractice.Model;
using Windows.Security.Cryptography.Core;
using MVVMPractice.Common;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using System.Net.Http;
using System.Diagnostics;
using Windows.Security.Authentication.Web;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Storage;
    
namespace TwitterOAuth
{
    public static class OAuth
    {

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

        private bool _authenticatedUser;
        public bool AuthenticatedUser
        {
            get { return _authenticatedUser; }
            set 
            { if (_authenticatedUser != value) { _authenticatedUser = value; } }
        }

        public TweetOAuth()
        {
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
            AuthenticatedUser = false;
            
        }

        public async Task AuthenticateTwitterByOAuth() 
        {
            Debug.WriteLine("In AuthenicateTwitterByOAuth Method");
            string OAuthRequest, OAuthResponse, twitterParams, twitterSignature;
            
            twitterParams = BuildOAuthParams(true);
            Debug.WriteLine(twitterParams);

            String signBase = HttpMethodType.GET.ToString() + "&"; //"GET&";
            signBase += Uri.EscapeDataString(TweetAPIConstants.RequestToken) + "&" + Uri.EscapeDataString(twitterParams);
            Debug.WriteLine(signBase);

            //Build and Get Twitter Signature
            twitterSignature = OAuthSignature(signBase, SignatureAuthType.ConsumerSecret);
            Debug.WriteLine(twitterSignature);

            //Build URL to Request Token
            OAuthRequest = TweetAPIConstants.RequestToken + "?" + twitterParams + "&"+ OAuth.OAuthTypes.OAuthSignatureString + Uri.EscapeDataString(twitterSignature);
            Debug.WriteLine(OAuthRequest);

            OAuthResponse = await OAuthRequestToken(OAuthRequest);
            Debug.WriteLine("Received Data: " + OAuthResponse);

            GetOAuthTokenKeys(OAuthResponse);
            await AuthorizeOAuthTokens();

            await GetOAuthAccessTokens();

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
            httpRequestMsg.RequestUri = new Uri(TweetAPIConstants.BearerToken, UriKind.Absolute);
            httpRequestMsg.Headers.Authorization = new AuthenticationHeaderValue("Basic", AppAuth.AppAuthTypes.AppAuthBasicToken);
            httpRequestMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");


            httpResponseMsg = await httpPostClient.SendAsync(httpRequestMsg);
            //return await response.Content.ReadAsStringAsync();

            if (httpResponseMsg.IsSuccessStatusCode)
            {
                Debug.WriteLine("Http Response returned Successful");
                authResponse = await httpResponseMsg.Content.ReadAsStringAsync();
                Debug.WriteLine(authResponse);


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
                        Debug.WriteLine("Got Access Token");
                        Debug.WriteLine(tokenDataType);
                        Debug.WriteLine(tokenKey);

                        AppAuth.AppAuthTypes.AppAuthBearerTokenKey = tokenKey;
                    }
                    else { AppAuth.AppAuthTypes.AppAuthBearerTokenString = tokenDataType; }
                }
   
            }
            else { this.AuthenticatedUser = false; }

            //return this.AuthenticatedUser;
        }

        public TweetAuthType AuthenticationType{get; set;}

        public string OAuthSignature(string SigBaseString, SignatureAuthType signType)
        {
            string signingKey = string.Empty; 
            Debug.WriteLine("In OAuthSignature function");

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

           // string signingKey = string.Format("{0}&{1}", consumerSecretKey, !string.IsNullOrEmpty(requestTokenSecretKey) ? requestTokenSecretKey : "");
            
            //signingKey = string.Format("{0}&{1}", OAuth.OAuthTypes.OAuthConsumerSecret, !string.IsNullOrEmpty(OAuth.OAuthTypes.OAuthAccessTokenSecretKey) ? OAuth.OAuthTypes.OAuthAccessTokenSecretKey : "");
           
            //IBuffer KeyBuffer = CryptographicBuffer.ConvertStringToBinary(OAuth.OAuthTypes.OAuthConsumerSecret + "&", BinaryStringEncoding.Utf8);
            IBuffer KeyBuffer = CryptographicBuffer.ConvertStringToBinary(signingKey, BinaryStringEncoding.Utf8);

            MacAlgorithmProvider HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);
            CryptographicKey MacKey = HmacSha1Provider.CreateKey(KeyBuffer);
            IBuffer DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(SigBaseString, BinaryStringEncoding.Utf8);
            IBuffer SignatureBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);
            String Signature = CryptographicBuffer.EncodeToBase64String(SignatureBuffer);
            Debug.WriteLine("Signed Signature: " + Signature);
            return Signature;

        }

        public async Task<string> OAuthRequestToken(string requestUrl)
        {
            Debug.WriteLine("In OAuthRequestToken function"); 

            HttpClient webClient = new HttpClient();
            string webResponse = String.Empty;

            Debug.WriteLine("Request Url: " + requestUrl); 

            try
            {
                //HttpClient httpClient = new HttpClient();
                //webResponse = await webClient.GetStringAsync(requestUrl);
                return await webClient.GetStringAsync(requestUrl);
                
            }
            catch (Exception Err)
            {
                Debug.WriteLine(Err);
                webResponse = Err.ToString();
                //rootPage.NotifyUser("Error getting data from server." + Err.Message, NotifyType.StatusMessage);
            }

            return null;
        
        }

        public string BuildOAuthParams(bool _doCallback)
        {
            
            string OAuthParameters = String.Empty;
            string Nonce = OAuthNonce();
            string timeStamp = OAuthTimeStamp(HttpMethodType.GET);

            OAuth.OAuthTypes.OAuthSignatureMethodKey = TweetAPIConstants.TweetSignatureMethod;
            if (_doCallback)
            {
                OAuthParameters += OAuth.OAuthTypes.OAuthCallbackString +
                    Uri.EscapeDataString(OAuth.OAuthTypes.OAuthCallbackKey == String.Empty ? TweetAPIConstants.NoCallBack : OAuth.OAuthTypes.OAuthCallbackKey) + "&";
            }
            OAuthParameters += OAuth.OAuthTypes.OAuthConsumerKeyString + OAuth.OAuthTypes.OAuthConsumerKey + "&";
            OAuthParameters += OAuth.OAuthTypes.OAuthNonceString + Nonce + "&";
            OAuthParameters += OAuth.OAuthTypes.OAuthSignatureMethodString + OAuth.OAuthTypes.OAuthSignatureMethodKey + "&";
            OAuthParameters += OAuth.OAuthTypes.OAuthTimestampString + timeStamp + "&";
            OAuthParameters += OAuth.OAuthTypes.OAuthVersionString + OAuth.OAuthTypes.OAuthVersionKey;

            
            Debug.WriteLine("Nonce: " + Nonce);
            Debug.WriteLine("TimeStamp; " + timeStamp);
            Debug.WriteLine("Parameters: " + OAuthParameters);

            return OAuthParameters;
        }

        

        public string OAuthNonce() 
        {
            Random randNum = new Random();
            Int32 nonce = randNum.Next(1000000000);
            return nonce.ToString();
            
        }

        public string OAuthTimeStamp(HttpMethodType httpMethod) 
        {
            string TimeStamp = String.Empty;

            if (httpMethod == HttpMethodType.GET) 
            {
                TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
                Debug.WriteLine("Rounded SinceEpoch: " + Math.Round(SinceEpoch.TotalSeconds));
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
                Debug.WriteLine(Err.ToString());
                
            }

            return null;
        }

        public void GetOAuthTokenKeys(string OAuthResponse)
        {
            Debug.WriteLine("In GetOAuthTokenKeys function"); 
            //String[] keyValPairs, splits;
            string OAuthToken = String.Empty, OAuthTokenSecret = String.Empty;

            //Cleaning up my code, only need 3 lines to replace 13 lines of code below
            Dictionary<string, string> oauthResult = OAuthResponse.Split('&').Select(param => param.Split('=')).ToDictionary(item => item[0], item=>item[1]);
            OAuth.OAuthTypes.OAuthRequestTokenKey = oauthResult["oauth_token"];
            OAuth.OAuthTypes.OAuthRequestTokenSecretKey = oauthResult["oauth_token_secret"];

            //if (OAuthResponse != null)
            //{

            //    keyValPairs = OAuthResponse.Split('&');

            //    for (int i = 0; i < keyValPairs.Length; i++)
            //    {
            //        splits = keyValPairs[i].Split('=');
            //        switch (splits[0])
            //        {
            //            case "oauth_token":
            //                OAuthToken = splits[1];
            //                break;
            //            case "oauth_token_secret":
            //                OAuthTokenSecret = splits[1];
            //                break;
            //        }
            //    }
            //}

            //OAuth.OAuthTypes.OAuthRequestTokenKey = !String.IsNullOrEmpty(OAuthToken) ? OAuthToken : String.Empty;
            //OAuth.OAuthTypes.OAuthRequestTokenSecretKey = !String.IsNullOrEmpty(OAuthTokenSecret) ? OAuthTokenSecret : String.Empty;

            Debug.WriteLine("Request Tokens Acquired, and AuthenticatedUser is " + this.AuthenticatedUser);
            
        }

        protected async Task AuthorizeOAuthTokens()
        {
            Debug.WriteLine("In AuthorizeOAuthToken function");

            HttpClient tweetClient = new HttpClient();
            string authorizeRequestUrl, authorizeResponse;
            Uri requestUri, callbackUri;
            //HttpResponseMessage authResponse;
            //HttpRequestMessage authRequest  //"https://api.twitter.com/oauth/authorize?oauth_token=" + oauth_token;

            authorizeRequestUrl = TweetAPIConstants.AuthorizeURL + "?" + OAuth.OAuthTypes.OAuthTokenString + OAuth.OAuthTypes.OAuthRequestTokenKey; 
            requestUri = new Uri(authorizeRequestUrl, UriKind.Absolute);
            callbackUri = new Uri(OAuth.OAuthTypes.OAuthCallbackKey);

            Debug.WriteLine("Navigating to: " + authorizeRequestUrl); // DebugPrint("Navigating to: " + TwitterUrl);
            
            WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                                WebAuthenticationOptions.None,
                                                                requestUri,
                                                                callbackUri);

            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
               Debug.WriteLine("Access Tokens Acquired, User Authorized and AuthenicatedUser is " + this.AuthenticatedUser);

                authorizeResponse = webAuthenticationResult.ResponseData.ToString();
                //IEnumerable<string> authData = authorizeResponse.Split('?').SelectMany((item) => item.Split('&').SelectMany((itm) => itm.Split('=')));
                Dictionary<string, string> authData = authorizeResponse.Split('?').ToList()[1].Split('&').Select(p => p.Split('=')).ToDictionary(param => param[0], param => param[1]);
                OAuth.OAuthTypes.OAuthAccessTokenKey = authData["oauth_token"];
                OAuth.OAuthTypes.OAuthVerifierKey = authData["oauth_verifier"];
                //OAuth.OAuthTypes.OAuthAccessTokenKey = authData.Select((item)).Where
                //OAuth.OAuthTypes.OAuthAccessTokenKey = authData.ElementAt<string>(1) == null ? String.Empty : authData.ElementAt<string>(2);
                //OAuth.OAuthTypes.OAuthVerifierKey = authData.ElementAt<string>(2) == null ? String.Empty : authData.ElementAt<string>(4);
                
            }
            else
            {
                this.AuthenticatedUser = false; Debug.WriteLine("User Not Authorized, In Tweet OAuth and AuthenicatedUser is " + this.AuthenticatedUser);
                throw new Exception(webAuthenticationResult.ResponseErrorDetail.ToString());
            }

        }

        protected async Task GetOAuthAccessTokens()
        {
            string tokenResponse; 

            OAuth.OAuthTypes.OAuthNonceKey = OAuthNonce();
            OAuth.OAuthTypes.OAuthTimestampKey = OAuthTimeStamp(HttpMethodType.GET);
            OAuth.OAuthTypes.OAuthSignatureMethodKey = TweetAPIConstants.TweetSignatureMethod;

            //string testVerifier = String.IsNullOrEmpty(oAuthVerifier.Text) ? String.Empty : oAuthVerifier.Text;

            string sigBaseStringParams = OAuth.OAuthTypes.OAuthConsumerKeyString + OAuth.OAuthTypes.OAuthConsumerKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthNonceString + OAuth.OAuthTypes.OAuthNonceKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthSignatureMethodString + OAuth.OAuthTypes.OAuthSignatureMethodKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthTimestampString + OAuth.OAuthTypes.OAuthTimestampKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthTokenString + OAuth.OAuthTypes.OAuthRequestTokenKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthVerifierString + OAuth.OAuthTypes.OAuthVerifierKey;
            sigBaseStringParams += "&" + OAuth.OAuthTypes.OAuthVersionString + OAuth.OAuthTypes.OAuthVersionKey; 

            string sigBaseString = "POST&";
            sigBaseString += Uri.EscapeDataString(TweetAPIConstants.AccessToken) + "&" + Uri.EscapeDataString(sigBaseStringParams);

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
                requestMsg.RequestUri = new Uri(TweetAPIConstants.AccessToken, UriKind.Absolute);
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
                //string oauth_token = null;
                //string oauth_token_secret = null;

                //string[] 
                Dictionary<string, string> oauthValPairs = tokenResponse.Split('&').Select(item => item.Split('=')).ToDictionary(p => p[0], p => p[1]);

                OAuth.OAuthTypes.OAuthAccessTokenKey =  oauthValPairs["oauth_token"];
                OAuth.OAuthTypes.OAuthAccessTokenSecretKey = oauthValPairs["oauth_token_secret"];
                TweetAPIConstants.APIUserID = oauthValPairs["user_id"];
                TweetAPIConstants.APIUserScreenName = oauthValPairs["screen_name"];

                //var keyValPairs = tokenResponse.Split('&').Select((item, index) => item.Split('='));
                //var oauthResult = keyValPairs.Select(item => (item[0]);
                    //ToDictionary(p => p[0].ToString()));

                //for (int i = 0; i < keyValPairs.Length; i++)
                //{
                //    String[] splits = keyValPairs[i].Split('=');
                //    switch (splits[0])
                //    {
                //        case "oauth_token":
                //            oauth_token = splits[1];
                //            break;
                //        case "oauth_token_secret":
                //            oauth_token_secret = splits[1];
                //            break;
                //    }
                //}

               // OAuth.OAuthTypes.OAuthAccessTokenKey = oauth_token;
                //OAuth.OAuthTypes.OAuthAccessTokenSecretKey = oauth_token_secret;
                
            }
        
        }

        public void StoreOAuthUserInfo() 
        {
            //ApplicationData.Current.LocalSettings.DeleteContainer("TwitterAPIMash");
            ApplicationDataContainer appLocalData = ApplicationData.Current.LocalSettings.CreateContainer("TwitterAPIMash", ApplicationDataCreateDisposition.Always);

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

            if (appLocalData.Values.ContainsKey("UserID")) { appLocalData.Values["UserID"] = TweetAPIConstants.APIUserID; }
            else { appLocalData.Values.Add("UserID", TweetAPIConstants.APIUserID); }

            if (appLocalData.Values.ContainsKey("UserScreenName")) { appLocalData.Values["UserScreenName"] = TweetAPIConstants.APIUserScreenName; }
            else { appLocalData.Values.Add("UserScreenName", TweetAPIConstants.APIUserScreenName); }            
        
        }

        public string AppAuthTokenEncoding() 
        {
            string encodedConsumerKey, encodedConsumerSecretKey, encodedAuthTokenKey;

            encodedConsumerKey = WebUtility.UrlEncode(AppAuth.AppAuthTypes.AppAuthConsumerKey);
            encodedConsumerSecretKey = WebUtility.UrlEncode(AppAuth.AppAuthTypes.AppAuthConsumerSecret);

            encodedAuthTokenKey = String.Concat(encodedConsumerKey, ":", encodedConsumerSecretKey);


            return Convert.ToBase64String(Encoding.UTF8.GetBytes(encodedAuthTokenKey));
            
        }

        //Removing for Final PRoject -- Test Function
        public async void Launch()
        {
            try
            {
                //
                // Acquiring a request token
                //
                //TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
                //Random Rand = new Random();
                String TwitterUrl = TweetAPIConstants.RequestToken;//"https://api.twitter.com/oauth/request_token";
                //Int32 Nonce = Rand.Next(1000000000);
                string SinceEpoch = OAuthTimeStamp(HttpMethodType.GET);
                string Nonce = OAuthNonce();

                Debug.WriteLine("Nonce: " + Nonce);

                //
                // Compute base signature string and sign it.
                //    This is a common operation that is required for all requests even after the token is obtained.
                //    Parameters need to be sorted in alphabetical order
                //    Keys and values should be URL Encoded.
                //
                string SigBaseStringParams = BuildOAuthParams(true);
                //String SigBaseStringParams = "oauth_callback=" + Uri.EscapeDataString(TweetAPIConstants.NoCallBack);
                //SigBaseStringParams += "&" + "oauth_consumer_key=" + OAuth.OAuthTypes.OAuthConsumerKey;
                //SigBaseStringParams += "&" + "oauth_nonce=" + Nonce.ToString();
                //SigBaseStringParams += "&" + "oauth_signature_method=" + TweetAPIConstants.TweetSignatureMethod;//HMAC-SHA1";
                //SigBaseStringParams += "&" + "oauth_timestamp=" + SinceEpoch;//SigBaseStringParams += "&" + "oauth_timestamp=" + Math.Round(SinceEpoch.TotalSeconds);
                //SigBaseStringParams += "&" + "oauth_version=1.0";
                String SigBaseString = "GET&";
                SigBaseString += Uri.EscapeDataString(TwitterUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);

                //Debug.WriteLine("TimeSpan: " + Math.Round(SinceEpoch.TotalSeconds));
                Debug.WriteLine("TimeSpan: " + SinceEpoch);
                Debug.WriteLine("Signature Base: " + SigBaseString);
                Debug.WriteLine("Parameters: " + SigBaseStringParams);

                String Signature = OAuthSignature(SigBaseString,SignatureAuthType.ConsumerSecret);
                Debug.WriteLine(Signature);

                TwitterUrl += "?" + SigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(Signature);

                Debug.WriteLine("Twitter URL: " + TwitterUrl);

                String GetResponse = await SendDataAsync(TwitterUrl);
                Debug.WriteLine("Received Data: " + GetResponse);

                if (GetResponse != null)
                {
                    String oauth_token = null;
                    String oauth_token_secret = null;
                    String[] keyValPairs = GetResponse.Split('&');

                    for (int i = 0; i < keyValPairs.Length; i++)
                    {
                        String[] splits = keyValPairs[i].Split('=');
                        switch (splits[0])
                        {
                            case "oauth_token":
                                oauth_token = splits[1];
                                break;
                            case "oauth_token_secret":
                                oauth_token_secret = splits[1];
                                break;
                        }
                    }

                   
                }
            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                //
                Debug.WriteLine(Error.ToString());
            }
        }		    
    }
}
