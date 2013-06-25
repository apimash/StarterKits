/*
File: about.xaml
Author: David Isbitski - @TheDaveDev, david.isbitski@microsoft.com, http://davedev.net
Last Mod: 6/25/2013
Description: Aboutpage class

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
using Microsoft.Phone.Tasks;

namespace WOWAPIKit
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }

        private void btnGithub_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://github.com/apimash", UriKind.Absolute);
            webBrowserTask.Show();
        }

        private void btnBlog_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://davedev.net", UriKind.Absolute);
            webBrowserTask.Show();
        }
    }
}