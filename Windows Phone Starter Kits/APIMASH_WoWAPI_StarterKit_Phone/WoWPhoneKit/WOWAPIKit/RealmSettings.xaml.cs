/*
File: realmsettings.xaml
Author: David Isbitski - @TheDaveDev, david.isbitski@microsoft.com, http://davedev.net
Last Mod: 6/25/2013
Description: User settings page

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

namespace WOWAPIKit
{
    public partial class RealmSettings : PhoneApplicationPage
    {
        public RealmSettings()
        {
            InitializeComponent();
            InitUserSettings();
        }

        private void InitUserSettings()
        {
            if (App.appSettings.Contains("realm"))
            {
                txtRealm.Text = (string)App.appSettings["realm"];
            }
        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (App.appSettings.Contains("realm"))
            {
                App.appSettings["realm"] = txtRealm.Text;
            }
            else
            {
                App.appSettings.Add("realm", txtRealm.Text);
            }

            var result = MessageBox.Show("Your Realm has been updated to '" + txtRealm.Text + "'.", "Changes Saved", MessageBoxButton.OK);

            App.allRealms = false;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}