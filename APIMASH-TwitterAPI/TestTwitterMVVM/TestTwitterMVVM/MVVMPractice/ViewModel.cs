using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MVVMPractice.Common;
using MVVMPractice.Model;
using System.Net;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TwitterOAuth;
using TwitterAPI;
using Windows.Storage;

namespace MVVMPractice.ViewModel
{
    public class TweetViewModel : BindableBase
    {
      #region PrivateVMVariables
        private string _screenName;
        private string _consumerKey;
        private string _consumerSecret;
        private Windows.UI.Xaml.Visibility _trendGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
        private Windows.UI.Xaml.Visibility _tweetGridVisible = Windows.UI.Xaml.Visibility.Collapsed;

        //Tweets Collection
        private ObservableCollection<Tweet> _userTweets;
        private ObservableCollection<Tweet> _userHomeTweets;
        private ObservableCollection<User> _userFollowers;
        private ObservableCollection<Trend> _globalTrendTweets;
        private ObservableCollection<Tweet> _searchTweets;
        private ObservableCollection<Tweet> _gridViewTweets;

        //Command Relay for Tweets
        private ICommand _getTweetByScreenNameCmd;
        private ICommand _getTweetByHomeTimelineCmd;
        private ICommand _getTweetByTrendCmd; 
        private ICommand _getTweetBySearchCmd;
        private int _twitterAuthType;
        private TweetOAuth oauthTwitter;
        private TweetAPICall tweetAPI;
      #endregion

      #region PublicVMVariables

        public Windows.UI.Xaml.Visibility TrendGridVisible 
        {
            get { return _trendGridVisible; }
            set 
            {
                _trendGridVisible = value;
                OnPropertyChanged("TrendGridVisible");
            }
        }

        public Windows.UI.Xaml.Visibility TweetGridVisible
        {
            get { return _tweetGridVisible; }
            set
            {
                _tweetGridVisible = value;
                OnPropertyChanged("TweetGridVisible");
            }
        }
 
        public int TwitterAuthType 
        {
            get { return _twitterAuthType; }
            set 
            { if (value != _twitterAuthType)
                { 
                    _twitterAuthType = value;
                    OnPropertyChanged("TwitterAuthType");
                }
            }
        }

        public string ConsumerKey 
        {
            set 
            {
                if (value != _consumerKey)
                {
                    _consumerKey = value;
                    OnPropertyChanged("ConsumerKey");

                }
            }
        }

        public string ConsumerSecret
        {
            set 
            {
                if (value != _consumerSecret)
                {
                    _consumerSecret = value;
                    OnPropertyChanged("ConsumerSecret");
                }
            }
        }

        public string UserScreenName
        {
            get { return _screenName; }
            set 
            {
                if (value != _screenName)
                { 
                    _screenName = value;
                    OnPropertyChanged("UserScreenName");
                }
            }
        }

        public ObservableCollection<Tweet> UserHomeTweets
        {
            get { return _userHomeTweets; }
            protected set
            {
                if (value != _userHomeTweets)
                {
                    _userHomeTweets = value;
                    OnPropertyChanged("UserHomeTweets");
                }
            }
        }

        //This one is just for testing
        public ObservableCollection<Tweet> GridViewTweets
        {
            get { return _gridViewTweets;}
            protected set 
            {
                if (value != _gridViewTweets)
                {
                    _gridViewTweets = value;
                    OnPropertyChanged("GridViewTweets");
                }
            }
        }

        public ObservableCollection<Tweet> UserTweets
        {
            get { return _userTweets; }
            protected set 
            {
                if (value != _userTweets)
                {
                    _userTweets = value;
                    OnPropertyChanged("UserTweets");
                }
            }
        }

        public ObservableCollection<Trend> GlobalTrendTweets 
        {
            get { return _globalTrendTweets; }
            protected set 
            {
                if (value != _globalTrendTweets)
                {
                    _globalTrendTweets = value;
                    OnPropertyChanged("GlobalTrendTweets");
                }
            
            }
        }

        public ObservableCollection<Tweet> SearchTweets
        {
            get { return _searchTweets; }
            protected set
            {
                if (value != _searchTweets)
                {
                    _searchTweets = value;
                    OnPropertyChanged("SearchTweets");
                }

            }
        }

        public ObservableCollection<User> UserFollowers 
        {
            get { return _userFollowers; }
            protected set 
            {
                if (value != _userFollowers)
                {
                    _userFollowers = value;
                    OnPropertyChanged("UserFollowers");
                }
            }
        }

      #endregion

      #region PublicCommandRelayMethods 
        public ICommand GetUserTweetsbyUser
        {
            get 
            {
               if (_getTweetByScreenNameCmd == null) 
               {  _getTweetByScreenNameCmd = new CommandRelay(param => GetUserTweets(_twitterAuthType), param => true);
               
               }            
                return _getTweetByScreenNameCmd;       
            }
        }
        
        public ICommand GetTweetsbyTrend
        {
            get
            {
                if (_getTweetByTrendCmd == null)
                {
                    _getTweetByTrendCmd = new CommandRelay(param => GetTrendTweets(_twitterAuthType), param => true);

                }
                return _getTweetByTrendCmd;
            }
        }

        public ICommand GetTweetsbyHomeTimeline
        {
            get
            {
                if (_getTweetByHomeTimelineCmd == null)
                {
                    _getTweetByHomeTimelineCmd = new CommandRelay(param => GetHomeTimelineTweets(_twitterAuthType), param => true);

                }
                return _getTweetByHomeTimelineCmd;
            }
        }

        public ICommand GetTweetsbySearch
        {
            get
            {
                if (_getTweetBySearchCmd == null)
                {
                    _getTweetBySearchCmd = new CommandRelay(param => GetSearchTweets(_twitterAuthType), param => true);

                }
                return _getTweetBySearchCmd;
            }
        }

      #endregion

      #region ViewModelMethods
        private async void GetUserTweets(int TwitAuthType)
        {

            TweetAuthType _selectedAuth;


            _selectedAuth = Convert.ToInt32(TweetAuthType.AppAuth) == TwitAuthType ? TweetAuthType.AppAuth : TweetAuthType.OAuth;

            //Set appropriate Grid - Binding to Visibility to keep all code in ViewModel not the best method, but quick and dirty for UI Test of API Data
            TrendGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            TweetGridVisible = Windows.UI.Xaml.Visibility.Visible;

            //Check to see if user has been authenticated already
            ApplicationDataContainer appLocalData = ApplicationData.Current.LocalSettings.CreateContainer("TwitterAPIMash", ApplicationDataCreateDisposition.Always);

            //Put this is a function to be used at each call
            if (appLocalData.Values.ContainsKey("OAuthAccessToken") && appLocalData.Values.ContainsKey("OAuthAccessSecret"))
            {
                OAuth.OAuthTypes.OAuthConsumerKey = appLocalData.Values["OAuthConsumerKey"].ToString();
                OAuth.OAuthTypes.OAuthConsumerSecret = appLocalData.Values["OAuthConsumerSecret"].ToString();
                OAuth.OAuthTypes.OAuthAccessTokenKey = appLocalData.Values["OAuthAccessToken"].ToString();
                OAuth.OAuthTypes.OAuthAccessTokenSecretKey = appLocalData.Values["OAuthAccessSecret"].ToString();
                OAuth.OAuthTypes.OAuthVerifierKey = appLocalData.Values["OAuthVerifier"].ToString();
                TweetAPIConstants.APIUserID = appLocalData.Values["UserID"].ToString();
                TweetAPIConstants.APIUserScreenName = appLocalData.Values["UserScreenName"].ToString();
            }
            else  //Put this is a function to be used at each call
            {
                //Instantiate Twitter OAuth object by Sending in Consumer Key and Consumer Secret, and AuthType
                oauthTwitter = new TweetOAuth(_consumerKey, _consumerSecret, _selectedAuth);

                //Authorize Twitter
                if (oauthTwitter.AuthenticatedUser == false)
                {
                    if (oauthTwitter.AuthenticationType == TweetAuthType.AppAuth)
                    {
                        await oauthTwitter.AuthenticateTwitterbyAppAuth();
                        Debug.WriteLine("Function Returned  and Authenicated User = " + oauthTwitter.AuthenticatedUser);

                    }
                    else
                    {
                        await oauthTwitter.AuthenticateTwitterByOAuth();
                        Debug.WriteLine("I am next line after AuthenicateTwitterByOAuth call and AuthenicatedUser " + oauthTwitter.AuthenticatedUser);
                    }

                }

                Debug.WriteLine("Outside of Auth function calls, Ready to call TweetAPI and AuthenticatedUser is " + oauthTwitter.AuthenticatedUser);
            }

            if (!String.IsNullOrEmpty(UserScreenName))
            {
                tweetAPI = new TweetAPICall();
                //Debug.WriteLine("Trying to get tweets and AuthUser is " + oauthTwitter.AuthenticatedUser);
                UserTweets =  await tweetAPI.TweetsByUser(UserScreenName);
                GridViewTweets = UserTweets;
            }
            else
            {
                throw new ArgumentNullException("No Twitter Screen Name Provided");
            }

            //GridViewTweets = UserTweets;

        }

        private async void GetHomeTimelineTweets(int TwitAuthType)
        {
            TweetAuthType _selectedAuth;

            _selectedAuth = Convert.ToInt32(TweetAuthType.AppAuth) == TwitAuthType ? TweetAuthType.AppAuth : TweetAuthType.OAuth;

            //Set appropriate Grid - Binding to Visibility to keep all code in ViewModel not the best method, but quick and dirty for UI Test of API Data
            TrendGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            TweetGridVisible = Windows.UI.Xaml.Visibility.Visible;

            //Check to see if user has been authenticated already
            ApplicationDataContainer appLocalData = ApplicationData.Current.LocalSettings.CreateContainer("TwitterAPIMash", ApplicationDataCreateDisposition.Always);

            //Put this is a function to be used at each call
            if (appLocalData.Values.ContainsKey("OAuthAccessToken") && appLocalData.Values.ContainsKey("OAuthAccessSecret"))
            {
                OAuth.OAuthTypes.OAuthConsumerKey = appLocalData.Values["OAuthConsumerKey"].ToString();
                OAuth.OAuthTypes.OAuthConsumerSecret = appLocalData.Values["OAuthConsumerSecret"].ToString();
                OAuth.OAuthTypes.OAuthAccessTokenKey = appLocalData.Values["OAuthAccessToken"].ToString();
                OAuth.OAuthTypes.OAuthAccessTokenSecretKey = appLocalData.Values["OAuthAccessSecret"].ToString();
                OAuth.OAuthTypes.OAuthVerifierKey = appLocalData.Values["OAuthVerifier"].ToString();
                TweetAPIConstants.APIUserID = appLocalData.Values["UserID"].ToString();
                TweetAPIConstants.APIUserScreenName = appLocalData.Values["UserScreenName"].ToString();
            }
            else  //Put this is a function to be used at each call
            {
                //Instantiate Twitter OAuth object by Sending in Consumer Key and Consumer Secret, and AuthType
                oauthTwitter = new TweetOAuth(_consumerKey, _consumerSecret, _selectedAuth);

                //Authorize Twitter
                if (oauthTwitter.AuthenticatedUser == false)
                {
                    if (oauthTwitter.AuthenticationType == TweetAuthType.AppAuth)
                    {
                        await oauthTwitter.AuthenticateTwitterbyAppAuth();
                        Debug.WriteLine("Function Returned  and Authenicated User = " + oauthTwitter.AuthenticatedUser);
                    }
                    else
                    {
                        await oauthTwitter.AuthenticateTwitterByOAuth();
                        Debug.WriteLine("I am next line after AuthenicateTwitterByOAuth call and AuthenicatedUser " + oauthTwitter.AuthenticatedUser);
                    }

                }

                Debug.WriteLine("Outside of Auth function calls, Ready to call TweetAPI and AuthenticatedUser is " + oauthTwitter.AuthenticatedUser);
            }

           try
           {
               tweetAPI = new TweetAPICall();
               UserHomeTweets = await tweetAPI.TweetsbyHomeTimeline();
               GridViewTweets = UserHomeTweets;
                //Debug.WriteLine("Trying to get tweets and AuthUser is " + oauthTwitter.AuthenticatedUser);
                //UserTweets = await tweetAPI.TweetsByUser(UserScreenName);
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }

        }

        private async void GetTrendTweets(int TwitAuthType)
        {
            TweetAuthType _selectedAuth;

            _selectedAuth = Convert.ToInt32(TweetAuthType.AppAuth) == TwitAuthType ? TweetAuthType.AppAuth : TweetAuthType.OAuth;

            //Set appropriate Grid - Binding to Visibility to keep all code in ViewModel not the best method, but quick and dirty for UI Test of API Data
            TweetGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            TrendGridVisible = Windows.UI.Xaml.Visibility.Visible;

            //Check to see if user has been authenticated already
            ApplicationDataContainer appLocalData = ApplicationData.Current.LocalSettings.CreateContainer("TwitterAPIMash", ApplicationDataCreateDisposition.Always);

            //Put this is a function to be used at each call
            if (appLocalData.Values.ContainsKey("OAuthAccessToken") && appLocalData.Values.ContainsKey("OAuthAccessSecret"))
            {
                OAuth.OAuthTypes.OAuthConsumerKey = appLocalData.Values["OAuthConsumerKey"].ToString();
                OAuth.OAuthTypes.OAuthConsumerSecret = appLocalData.Values["OAuthConsumerSecret"].ToString();
                OAuth.OAuthTypes.OAuthAccessTokenKey = appLocalData.Values["OAuthAccessToken"].ToString();
                OAuth.OAuthTypes.OAuthAccessTokenSecretKey = appLocalData.Values["OAuthAccessSecret"].ToString();
                OAuth.OAuthTypes.OAuthVerifierKey = appLocalData.Values["OAuthVerifier"].ToString();
                TweetAPIConstants.APIUserID = appLocalData.Values["UserID"].ToString();
                TweetAPIConstants.APIUserScreenName = appLocalData.Values["UserScreenName"].ToString();
            }
            else  //Put this is a function to be used at each call
            {
                //Instantiate Twitter OAuth object by Sending in Consumer Key and Consumer Secret, and AuthType
                oauthTwitter = new TweetOAuth(_consumerKey, _consumerSecret, _selectedAuth);

                //Authorize Twitter
                if (oauthTwitter.AuthenticatedUser == false)
                {
                    if (oauthTwitter.AuthenticationType == TweetAuthType.AppAuth)
                    {
                        await oauthTwitter.AuthenticateTwitterbyAppAuth();
                        Debug.WriteLine("Function Returned  and Authenicated User = " + oauthTwitter.AuthenticatedUser);
                    }
                    else
                    {
                        await oauthTwitter.AuthenticateTwitterByOAuth();
                        Debug.WriteLine("I am next line after AuthenicateTwitterByOAuth call and AuthenicatedUser " + oauthTwitter.AuthenticatedUser);
                    }

                }

                Debug.WriteLine("Outside of Auth function calls, Ready to call TweetAPI and AuthenticatedUser is " + oauthTwitter.AuthenticatedUser);
            }

            try
            {
                tweetAPI = new TweetAPICall();
                GlobalTrendTweets = await tweetAPI.TweetsbyTrend(null);
                //GridViewTweets = GlobalTrendTweets;
                //Debug.WriteLine("Trying to get tweets and AuthUser is " + oauthTwitter.AuthenticatedUser);
                //UserTweets = await tweetAPI.TweetsByUser(UserScreenName);
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }

        }

        private async void GetSearchTweets(int TwitAuthType)
        {
            TweetAuthType _selectedAuth;

            _selectedAuth = Convert.ToInt32(TweetAuthType.AppAuth) == TwitAuthType ? TweetAuthType.AppAuth : TweetAuthType.OAuth;

            //Set appropriate Grid - Binding to Visibility to keep all code in ViewModel not the best method, but quick and dirty for UI Test of API Data
            TrendGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            TweetGridVisible = Windows.UI.Xaml.Visibility.Visible;

            //Check to see if user has been authenticated already
            ApplicationDataContainer appLocalData = ApplicationData.Current.LocalSettings.CreateContainer("TwitterAPIMash", ApplicationDataCreateDisposition.Always);

            //Put this is a function to be used at each call
            if (appLocalData.Values.ContainsKey("OAuthAccessToken") && appLocalData.Values.ContainsKey("OAuthAccessSecret"))
            {
                OAuth.OAuthTypes.OAuthConsumerKey = appLocalData.Values["OAuthConsumerKey"].ToString();
                OAuth.OAuthTypes.OAuthConsumerSecret = appLocalData.Values["OAuthConsumerSecret"].ToString();
                OAuth.OAuthTypes.OAuthAccessTokenKey = appLocalData.Values["OAuthAccessToken"].ToString();
                OAuth.OAuthTypes.OAuthAccessTokenSecretKey = appLocalData.Values["OAuthAccessSecret"].ToString();
                OAuth.OAuthTypes.OAuthVerifierKey = appLocalData.Values["OAuthVerifier"].ToString();
                TweetAPIConstants.APIUserID = appLocalData.Values["UserID"].ToString();
                TweetAPIConstants.APIUserScreenName = appLocalData.Values["UserScreenName"].ToString();
            }
            else  //Need to clean this up and put this is a function to be used at each call
            {
                //Instantiate Twitter OAuth object by Sending in Consumer Key and Consumer Secret, and AuthType
                oauthTwitter = new TweetOAuth(_consumerKey, _consumerSecret, _selectedAuth);

                //Authorize Twitter 
                if (oauthTwitter.AuthenticatedUser == false)
                {
                    if (oauthTwitter.AuthenticationType == TweetAuthType.AppAuth)
                    {
                        await oauthTwitter.AuthenticateTwitterbyAppAuth();
                        Debug.WriteLine("Function Returned  and Authenicated User = " + oauthTwitter.AuthenticatedUser);
                    }
                    else
                    {
                        await oauthTwitter.AuthenticateTwitterByOAuth();
                        Debug.WriteLine("I am next line after AuthenicateTwitterByOAuth call and AuthenicatedUser " + oauthTwitter.AuthenticatedUser);
                    }

                }

                Debug.WriteLine("Outside of Auth function calls, Ready to call TweetAPI and AuthenticatedUser is " + oauthTwitter.AuthenticatedUser);
            }

            if (!string.IsNullOrEmpty(UserScreenName))
            {
                tweetAPI = new TweetAPICall();
                SearchTweets = await tweetAPI.TweetsbySearch(UserScreenName);
                GridViewTweets = SearchTweets;
                //Debug.WriteLine("Trying to get tweets and AuthUser is " + oauthTwitter.AuthenticatedUser);
                //UserTweets = await tweetAPI.TweetsByUser(UserScreenName);
            }
            else
            {
                throw new Exception("Search Query Cannot Be Empty");
            }


        }
      #endregion

    }
    
}
