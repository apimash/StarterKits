using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TwitterAPIWin8Lib.APICalls;
using TwitterAPIWin8Lib.APICommon;
using TwitterAPIWin8Lib.Model;
using TwitterAPIWin8Lib.TwitterOAuth;
using Windows.Storage;


namespace TwitterAPIWin8Lib.ViewModel
{
    public class TweetViewModel : BindableBase
    {
        #region PrivateVMVariables
        private string _twitterSearchText;
        private string _consumerKey;
        private string _consumerSecret;
        private string _authenticatedUser;
        private bool AuthenticatedUserStatus;

        //Windows 8 Grid Manipulation
        //This is just so that I can show different Grid types and Binding in Xaml for Ideas 
        //(BEST PRACTICE: Create Item Templates in Standard Styles.xaml and use ItemTemplate Selector)
        private Windows.UI.Xaml.Visibility _trendGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
        private Windows.UI.Xaml.Visibility _tweetGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
        private Windows.UI.Xaml.Visibility _userGridVisible = Windows.UI.Xaml.Visibility.Collapsed;

        //Tweets Collection
        private User _currentTwitterUser;
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
        private ICommand _getUserFollowersbyUserNameCmd;
        private int _twitterAuthType;
        private TweetOAuth oauthTwitter;
        private TweetAPICall tweetAPI;
        #endregion

        #region Public_ViewModel_AuthVariables

        public int TwitterAuthType
        {
            get { return _twitterAuthType; }
            set { SetProperty<int>(ref _twitterAuthType, value); }
        }

        public string ConsumerKey
        {
            set { SetProperty<string>(ref _consumerKey, value); }
        }

        public string ConsumerSecret
        {
            set { SetProperty<string>(ref _consumerSecret, value); }
        }

        public string TwitterSearchText
        {
            get { return _twitterSearchText; }
            set { SetProperty<string>(ref _twitterSearchText, value); }
        }

        #endregion

        #region TwitterAPI_ViewModel_Collections

        //ViewModel Observable Collections for Twitter API Return Types
        public ObservableCollection<Tweet> UserHomeTweets
        {
            get { return _userHomeTweets; }
            protected set { SetProperty<ObservableCollection<Tweet>>(ref _userHomeTweets, value); }
            
        }

        //This one is just for testing
        public ObservableCollection<Tweet> GridViewTweets
        {
            get { return _gridViewTweets; }
            protected set { SetProperty<ObservableCollection<Tweet>>(ref _gridViewTweets, value); }
            
        }

        public ObservableCollection<Tweet> UserTweets
        {
            get { return _userTweets; }
            protected set { SetProperty<ObservableCollection<Tweet>>(ref _userTweets, value); } 
            
        }

        public ObservableCollection<Trend> GlobalTrendTweets
        {
            get { return _globalTrendTweets; }
            protected set { SetProperty<ObservableCollection<Trend>>(ref _globalTrendTweets, value); }
        }

        public ObservableCollection<Tweet> SearchTweets
        {
            get { return _searchTweets; }
            protected set { SetProperty<ObservableCollection<Tweet>>(ref _searchTweets, value); }
        }

        public ObservableCollection<User> UserFollowers
        {
            get { return _userFollowers; }
            protected set { SetProperty<ObservableCollection<User>>(ref _userFollowers, value);  }
        }

        public User CurrentTwitterUser 
        {
            get { return _currentTwitterUser;  }
            protected set { SetProperty<User>(ref _currentTwitterUser, value); }
        }
        #endregion  

        #region PublicCommandRelayMethods
        public ICommand GetUserTweetsbyUser
        {
            get
            {
                if (_getTweetByScreenNameCmd == null)
                    _getTweetByScreenNameCmd = new CommandRelay(param => GetUserTweets(_twitterAuthType), param => true);

                return _getTweetByScreenNameCmd;
            }
        }

        public ICommand GetTweetsbyTrend
        {
            get
            {
                if (_getTweetByTrendCmd == null)
                _getTweetByTrendCmd = new CommandRelay(param => GetTrendTweets(_twitterAuthType), param => true);

                return _getTweetByTrendCmd;
            }
        }

        public ICommand GetTweetsbyHomeTimeline
        {
            get
            {   
                if (_getTweetByHomeTimelineCmd == null)
                _getTweetByHomeTimelineCmd = new CommandRelay(param => GetHomeTimelineTweets(_twitterAuthType), param => true);

                return _getTweetByHomeTimelineCmd;
            }
        }

        public ICommand GetTweetsbySearch
        {
            get
            {
                if (_getTweetBySearchCmd == null)
                _getTweetBySearchCmd = new CommandRelay(param => GetSearchTweets(_twitterAuthType), param => true);
                
                return _getTweetBySearchCmd;
            }
        }

        public ICommand GetFollowersbyUserScreenName
        {
            get
            {
                if (_getUserFollowersbyUserNameCmd == null)
                    _getUserFollowersbyUserNameCmd = new CommandRelay(param => GetUserFollowers(_twitterAuthType), param => true);
                
                return _getUserFollowersbyUserNameCmd;
            }
        }

        public Windows.UI.Xaml.Visibility TrendGridVisible
        {
            get { return _trendGridVisible; }
            set { SetProperty<Windows.UI.Xaml.Visibility>(ref _trendGridVisible, value); }
        }

        public Windows.UI.Xaml.Visibility TweetGridVisible
        {
            get { return _tweetGridVisible; }
            set { SetProperty<Windows.UI.Xaml.Visibility>(ref _tweetGridVisible, value);  }
        }

        public Windows.UI.Xaml.Visibility UserGridVisible
        {
            get { return _userGridVisible; }
            set { SetProperty<Windows.UI.Xaml.Visibility>(ref _userGridVisible, value); }
        }
        #endregion

        #region Private_ViewModel_Methods
        /// <summary>
        /// Verify if User is Authenticated for use in Twitter
        /// </summary>
        /// <returns></returns>
        private bool VerifyAuthenticatedUser() 
        {   
            bool VerificationStatus = false; 
            //The Twitter Local Settings Storage
            ApplicationDataContainer twitterAPISettingsData;

            //Current App Local Settings Storage - variable added for code clarity only 
            ApplicationDataContainer currentAppLocalSettings = ApplicationData.Current.LocalSettings;

            //currentAppLocalSettings.DeleteContainer(CommonProperties.TwitterAPI_DataStorage);

            //Check to see if a TwitterAPIMash Settings Container already exists
            if (currentAppLocalSettings.Containers.ContainsKey(CommonProperties.TwitterAPI_DataStorage))
            {
                twitterAPISettingsData = currentAppLocalSettings.Containers[CommonProperties.TwitterAPI_DataStorage];

                //User is only truly verified if these keys are present in container
                VerificationStatus = twitterAPISettingsData.Values.ContainsKey("OAuthAccessToken") && twitterAPISettingsData.Values.ContainsKey("OAuthAccessSecret");
                
                if (VerificationStatus)
                {
                     OAuth.OAuthTypes.OAuthConsumerKey = twitterAPISettingsData.Values["OAuthConsumerKey"].ToString(); 
                    OAuth.OAuthTypes.OAuthConsumerSecret = twitterAPISettingsData.Values["OAuthConsumerSecret"].ToString();
                    OAuth.OAuthTypes.OAuthAccessTokenKey = twitterAPISettingsData.Values["OAuthAccessToken"].ToString();
                    OAuth.OAuthTypes.OAuthAccessTokenSecretKey = twitterAPISettingsData.Values["OAuthAccessSecret"].ToString();
                    OAuth.OAuthTypes.OAuthVerifierKey = twitterAPISettingsData.Values["OAuthVerifier"].ToString();
                    TweetAPIConstants.APIUserID = twitterAPISettingsData.Values["UserID"].ToString();
                    TweetAPIConstants.APIUserScreenName = twitterAPISettingsData.Values["UserScreenName"].ToString();
                    TweetAuthConstants.OAuthUserID = twitterAPISettingsData.Values["UserID"].ToString();
                    TweetAuthConstants.OAuthUserScreenName = twitterAPISettingsData.Values["UserScreenName"].ToString();
                    _authenticatedUser = TweetAPIConstants.APIUserScreenName;
                }
            }

            return VerificationStatus; 
        }

        /// <summary>
        /// Authenticate the current user for use in Twitter
        /// </summary>
        /// <param name="AuthType"></param>
        /// <returns></returns>
        private async Task<bool> AuthenticateTwitterUser(int AuthType) 
        {
            
            bool AuthenticationStatus = false;
            TweetAuthType _selectedAuth = Convert.ToInt32(TweetAuthType.AppAuth) == AuthType ? TweetAuthType.AppAuth : TweetAuthType.OAuth;

            oauthTwitter = new TweetOAuth(_consumerKey, _consumerSecret, _selectedAuth);

            try
            {
                if (oauthTwitter.AuthenticationType == TweetAuthType.AppAuth) await oauthTwitter.AuthenticateTwitterbyAppAuth();
                else await oauthTwitter.AuthenticateTwitterByOAuth();
                AuthenticationStatus = true;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }

            return AuthenticationStatus;
        }

        /// <summary>
        /// Retrieve Tweets for a Specific User Screen Name
        /// </summary>
        /// <param name="TwitAuthType"></param>
        private async void GetUserTweets(int TwitAuthType)
        {
            //Verify if User Authenticated by Twitter
            AuthenticatedUserStatus = VerifyAuthenticatedUser();

            //If User Not already verified by Twitter; Authenticate and Set updated User Auth State
            if (!AuthenticatedUserStatus) AuthenticatedUserStatus = await AuthenticateTwitterUser(TwitAuthType);

            //Set appropriate Grid - Binding to Visibility to keep all code in ViewModel not the best method, but quick and dirty for UI Test of API Data
            TrendGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            TweetGridVisible = Windows.UI.Xaml.Visibility.Visible;
            UserGridVisible = Windows.UI.Xaml.Visibility.Collapsed;

            //Do verification
            if (!String.IsNullOrEmpty(TwitterSearchText)) 
            {
                tweetAPI = new TweetAPICall();
                //Debug.WriteLine("Trying to get tweets and AuthUser is " + oauthTwitter.AuthenticatedUser);
                UserTweets = await tweetAPI.TweetsByUser(TwitterSearchText);
                GridViewTweets = UserTweets;
            }
            else
            {
                throw new ArgumentNullException("No Twitter Screen Name Provided");
            }

            //GridViewTweets = UserTweets;

        }

        /// <summary>
        /// Retrieve Tweets for the Twitter User (Home Timeline Tweets)
        /// </summary>
        /// <param name="TwitAuthType"></param>
        private async void GetHomeTimelineTweets(int TwitAuthType)
        {
            //Verify if User Authenticated by Twitter
            AuthenticatedUserStatus = VerifyAuthenticatedUser();

            //If User Not already verified by Twitter; Authenticate and Set updated User Auth State
            if (!AuthenticatedUserStatus) AuthenticatedUserStatus = await AuthenticateTwitterUser(TwitAuthType);

            //Set appropriate Grid - Binding to Visibility to keep all code in ViewModel not the best method, but quick and dirty for UI Test of API Data
            TrendGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            TweetGridVisible = Windows.UI.Xaml.Visibility.Visible;
            UserGridVisible = Windows.UI.Xaml.Visibility.Collapsed;

            try
            {
                tweetAPI = new TweetAPICall();
                UserHomeTweets = await tweetAPI.TweetsbyHomeTimeline();
                GridViewTweets = UserHomeTweets;
             
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }

        }

        /// <summary>
        /// Retrieve Tweets Trend Topics
        /// </summary>
        /// <param name="TwitAuthType"></param>
        private async void GetTrendTweets(int TwitAuthType)
        {
            //Verify if User Authenticated by Twitter
            AuthenticatedUserStatus = VerifyAuthenticatedUser();

            //If User Not already verified by Twitter; Authenticate and Set updated User Auth State
            if (!AuthenticatedUserStatus) AuthenticatedUserStatus = await AuthenticateTwitterUser(TwitAuthType);

           //Set appropriate Grid - Binding to Visibility to keep all code in ViewModel not the best method, but quick and dirty for UI Test of API Data
            TweetGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            TrendGridVisible = Windows.UI.Xaml.Visibility.Visible;
            UserGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            
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

        /// <summary>
        /// Retrieve Search by Topic Tweets
        /// </summary>
        /// <param name="TwitAuthType"></param>
        private async void GetSearchTweets(int TwitAuthType)
        {
            //Verify if User Authenticated by Twitter
            AuthenticatedUserStatus = VerifyAuthenticatedUser();

            //If User Not already verified by Twitter; Authenticate and Set updated User Auth State
            if (!AuthenticatedUserStatus) AuthenticatedUserStatus = await AuthenticateTwitterUser(TwitAuthType);

            //Set appropriate Grid - Binding to Visibility to keep all code in ViewModel not the best method, but quick and dirty for UI Test of API Data
            TrendGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            TweetGridVisible = Windows.UI.Xaml.Visibility.Visible;
            UserGridVisible = Windows.UI.Xaml.Visibility.Collapsed;

            if (!string.IsNullOrEmpty(TwitterSearchText))
            {
                tweetAPI = new TweetAPICall();
                SearchTweets = await tweetAPI.TweetsbySearch(TwitterSearchText);
                GridViewTweets = SearchTweets;
            }
            else
            { throw new Exception("Search Query Cannot Be Empty"); }


        }

        private async void GetUserFollowers(int TwitAuthType)
        {
            //Verify if User Authenticated by Twitter
            AuthenticatedUserStatus = VerifyAuthenticatedUser();

            //If User Not already verified by Twitter; Authenticate and Set updated User Auth State
            if (!AuthenticatedUserStatus) AuthenticatedUserStatus = await AuthenticateTwitterUser(TwitAuthType);

            //Set appropriate Grid - Binding to Visibility to keep all code in ViewModel not the best method, but quick and dirty for UI Test of API Data
            TrendGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            TweetGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            UserGridVisible = Windows.UI.Xaml.Visibility.Visible;

            //Do verification
            _twitterSearchText = String.IsNullOrEmpty(TwitterSearchText) ? _authenticatedUser : TwitterSearchText;
            if (!String.IsNullOrEmpty(TwitterSearchText))
            {
                tweetAPI = new TweetAPICall();
                //Debug.WriteLine("Trying to get tweets and AuthUser is " + oauthTwitter.AuthenticatedUser);
                UserFollowers = await tweetAPI.FollowersByUser(TwitterSearchText);
            }
            else
            {
                throw new ArgumentNullException("No Twitter Screen Name Provided");
            }
        }

        private async void GetUserInformation(int TwitAuthType) 
        {
            //Verify if User Authenticated by Twitter
            AuthenticatedUserStatus = VerifyAuthenticatedUser();

            //If User Not already verified by Twitter; Authenticate and Set updated User Auth State
            if (!AuthenticatedUserStatus) AuthenticatedUserStatus = await AuthenticateTwitterUser(TwitAuthType);

            //Set appropriate Grid - Binding to Visibility to keep all code in ViewModel not the best method, but quick and dirty for UI Test of API Data
            TrendGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            TweetGridVisible = Windows.UI.Xaml.Visibility.Collapsed;
            UserGridVisible = Windows.UI.Xaml.Visibility.Visible;
             
            //Do verification
            if (!String.IsNullOrEmpty(TwitterSearchText))
            {
                tweetAPI = new TweetAPICall();
                //Debug.WriteLine("Trying to get tweets and AuthUser is " + oauthTwitter.AuthenticatedUser);
                UserFollowers = await tweetAPI.FollowersByUser(TwitterSearchText);
                GridViewTweets = UserTweets;
            }
            else
            {
                throw new ArgumentNullException("No Twitter Screen Name Provided");
            }
        }
        #endregion

    }
}


