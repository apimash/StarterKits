//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// http://opensource.org/licenses/ms-pl
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************


using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;


using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace APIMASH_Univision_StarterKit
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class ItemsPage : APIMASH_Univision_StarterKit.Common.LayoutAwarePage
    {
        public ItemsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]
            FeedDataSource feedDataSource = (FeedDataSource)App.Current.Resources["feedDataSource"];

            if (feedDataSource != null)
            {
                this.DefaultViewModel["Items"] = feedDataSource.Feeds;
            }
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the split page, configuring the new page
            // by passing the title of the clicked item as a navigation parameter
            if (e.ClickedItem != null)
            {
                string title = ((FeedData)e.ClickedItem).Title;
                this.Frame.Navigate(typeof(SplitPage), title);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            // Load the results of the query in the different channels.
            Add_Feed(sender, e, nameInput.Text.ToUpper() + Globals.UNIVISION_CHANNEL_ENTERTAINMENT_TITLE, 
                Globals.UNIVISION_CHANNEL_ENTERTAINMENT);
            Add_Feed(sender, e, nameInput.Text.ToUpper() + Globals.UNIVISION_CHANNEL_NOTICIAS_TITLE, 
                Globals.UNIVISION_CHANNEL_NOTICIAS);//"noticias");


        }

        private async void Add_Feed(object sender, RoutedEventArgs e, string FeedTitle, string Channel)
        {
            Windows.Storage.ApplicationDataContainer roamingSettings =
                Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["searchValue"] = FeedTitle;//nameInput.Text;

            string searchWord = nameInput.Text;

            string newSearch = Globals.UNIVISION_URL_BASE
            + Channel                           //Univision Channel that is being queried
            + Globals.UNIVISION_TAGS_PORTION  
            + searchWord                        //New search entered by the user
            + Globals.UNIVISION_ENCODING_PORTION
            + Globals.UNIVISION_API_KEY;        //Change this value in Globals.cs

            FeedDataSource feedDataSource = (FeedDataSource)App.Current.Resources["feedDataSource"];

            if (feedDataSource != null)
            {
                await feedDataSource.GetOneFeedsAsync(newSearch, FeedTitle);
            }


        }


        private void NameInput_TextChanged(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer roamingSettings =
                Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["searchValue"] = nameInput.Text;

        }
    }
}