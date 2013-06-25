/*
File: mainpage.xaml
Author: David Isbitski - @TheDaveDev, david.isbitski@microsoft.com, http://davedev.net
Last Mod: 6/25/2013
Description: Main page class

Latest Source: http://github.com/disbitski/win8wowapikit or http://github.com/apimash/StarterKits

Menu Bar Icons: http://www.pedrolamas.com/windows-phone/windows-phone-8-application-bar-icons/
 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;

namespace WOWAPIKit
{
    public partial class MainPage : PhoneApplicationPage
    {

        // Url of Home page
        private string uriRealmsAll = "/Html/index.html";
        private string uriRealmsSingle = "/Html/realmStatus.html";
     
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            InitSettings();

            Browser.IsScriptEnabled = true;
        }

        //Get server settings
        private void InitSettings() 
        {
            if (App.appSettings.Contains("realm"))
            {
                App.userRealm = (string)App.appSettings["realm"];
            }
            else
            {
                App.userRealm = App.defaultRealm;
                App.appSettings.Add("realm", App.defaultRealm);
            }
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.allRealms) 
            {
                Browser.Navigate(new Uri(uriRealmsAll, UriKind.Relative));
            }
            else
            {
                Browser.Navigate(new Uri(uriRealmsSingle, UriKind.Relative));
            }

        }

     
        private void btnAbout_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void btnSettings_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/RealmSettings.xaml", UriKind.Relative));
        }

        private void btnRealm_Click(object sender, System.EventArgs e)
        {
            App.allRealms = false;
            Browser.Navigate(new Uri(uriRealmsSingle, UriKind.Relative));
          }

        private void btnAllRealms_Click(object sender, System.EventArgs e)
        {
            App.allRealms = true;
            Browser.Navigate(new Uri(uriRealmsAll, UriKind.Relative));
        }

        private void Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (!App.allRealms)
            {
                Browser.IsScriptEnabled = true;
                String[] realm = new String[1];
                realm[0] = HttpUtility.UrlEncode(App.userRealm);
                Browser.InvokeScript("getRealmStatus", realm[0]);

            }
        }

    }
}
