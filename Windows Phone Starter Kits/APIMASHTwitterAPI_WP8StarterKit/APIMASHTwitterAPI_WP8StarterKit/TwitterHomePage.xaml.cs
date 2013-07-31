using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TwitterAPIWinPhone8Lib.ViewModel;
using APIMASHTwitterAPI_WP8StarterKit.Resources;
//using APIMASHTwitterAPI_WP8Lib.APICommon.UIHelper;

namespace APIMASHTwitterAPI_WP8StarterKit
{
    public partial class TwitterHomePage : PhoneApplicationPage
    {
        public static TwitterHomePage currentPage;
       

        public TwitterHomePage()
        {
            InitializeComponent();
            currentPage = this;
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            TweetViewModel myViewModel = new TweetViewModel();
            myViewModel.TwitterAuthType = 2; //Authentication Type: TweetAuthType --> AppAuth = 1, OAuth = 2 
            myViewModel.ConsumerKey = AppResources.AppKey;
            myViewModel.ConsumerSecret = AppResources.AppSecret;
            this.DataContext = myViewModel;
            
        }
       
    }

  
}