using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TwitterAPIWinPhone8Lib.TwitterOAuth;
using TwitterAPIWinPhone8Lib.APICommon;
using APIMASHTwitterAPI_WP8StarterKit.Resources;

namespace APIMASHTwitterAPI_WP8StarterKit
{
    public partial class TwitterAuthPage : PhoneApplicationPage
    {
        TweetOAuth _twitterOAuth;
        
        public TwitterAuthPage()
        {
            InitializeComponent();

            _twitterOAuth = new TweetOAuth(AppResources.AppKey, AppResources.AppSecret, TweetAuthType.OAuth);
            TwitterWebAuth.Navigated += TwitterWebAuth_Navigated;
            //TwitterWebAuth.Navigated += _twitterOAuth.AuthWebCallBack;

        }

       protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
           base.OnNavigatedTo(e);

           System.IO.IsolatedStorage.IsolatedStorageSettings authUserData = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
           string dataKey = CommonProperties.TwitterAPI_DataStorage.ToString();
           
           if (authUserData.Contains(CommonProperties.TwitterAPI_DataStorage.ToString())) this.NavigationService.Navigate(new Uri("/TwitterHomePage.xaml",UriKind.Relative));
           else await _twitterOAuth.AuthenticateTwitterByOAuth(this.TwitterWebAuth);

        }

       protected override void OnNavigatedFrom(NavigationEventArgs e)
       {
           base.OnNavigatedFrom(e);
           this.NavigationService.RemoveBackEntry();
       }

       async void TwitterWebAuth_Navigated(object sender, NavigationEventArgs e)
       {
           if (!e.Uri.AbsoluteUri.Contains(TweetAuthConstants.AuthorizeURL))
            {
               
               TwitterWebAuth.Visibility = System.Windows.Visibility.Collapsed;
                
                 ///Issue to Note:
                 ///Doesn't do this in Windows 8 just in Windows Phone 8 it adds as /default.aspx at the end of query string. Very weird
                 ///can't find documentation of why this would happen.  Tested code in Windows 8 project calling from WebAuthenticalResult with 
                 ///same Twitter class called it doesn't add this extra /default.aspx.  Code in if below is to correct this before sending to the 
                 ///Twitter API class so that resolves the return tokens in the same way as in Windows 8 does before calling Twitter API class object 
                 ///as it seems to be a UI Control issue vs. API issue
                string tokenResult = String.Empty;

                if (e.Uri.Query.Contains("/"))
                {   tokenResult = e.Uri.Query.Remove(e.Uri.Query.IndexOf("/")); }
                else tokenResult = e.Uri.Query;

                await _twitterOAuth.RetrieveWebOAuthTokens(tokenResult);

                this.NavigationService.Navigate(new Uri("/TwitterHomePage.xaml",UriKind.Relative));

            }
        }
        
    }
}