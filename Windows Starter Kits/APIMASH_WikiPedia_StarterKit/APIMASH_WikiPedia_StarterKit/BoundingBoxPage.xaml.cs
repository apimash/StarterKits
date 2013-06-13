// LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt   <== yes, there's a space in it, dont ask....
// APIMash - http://bit.ly/apimash
// Joe Healy / jhealy@microsoft.com / josephehealy@hotmail.com / @devfish

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using APIMASH_WikiPediaLib.geonamesHelpers;
using APIMASH_WikiPedia_StarterKit.Common;

namespace APIMASH_WikiPedia_StarterKit
{
    public sealed partial class BoundingBoxPage : APIMASH_WikiPedia_StarterKit.Common.LayoutAwarePage
    {
        APIMASHLib.APIMASHInvoke api_findNearbyPlaces;
        APIMASH_WikiPediaLib.APIMASH_geonamesCollection _nearbyplaces;
        MsgHelper m_msghelper;

        public BoundingBoxPage()
        {
            this.InitializeComponent();

            api_findNearbyPlaces = new APIMASHLib.APIMASHInvoke();
            api_findNearbyPlaces.OnResponse += api_findNearbyPlaces_OnResponse;
            _nearbyplaces = new APIMASH_WikiPediaLib.APIMASH_geonamesCollection();

            m_msghelper = new MsgHelper(TextBlock_Msg);
            m_msghelper.clr();
            m_msghelper.msg("initialized");
            m_msghelper.abouttextblock();

            this.Loaded += BoundingBoxPage_Loaded;
        }

        void BoundingBoxPage_Loaded(object sender, RoutedEventArgs e)
        {
            footer.SetStatus("Loaded...");
        }

        #region "STATE"
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
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
        #endregion 
    

        private void Button_Invoke_Click(object sender, RoutedEventArgs e)
        {
            Invoke();
        }

        private void Invoke()
        {
            //string apicall = @"http://api.geonames.org/wikipediaBoundingBoxJSON?north=44.1&south=-9.9&east=-22.4&west=55.2&username=devfish";
            string _username = APIMASHGlobals.Instance.UserID;
            if ( _username.Length<=0) 
            {
                MessageDialogHelper.ShowMsg( "username", "valid geonames username is required to be set, use the CHARMS=>SETTINGS menu to enter yours" );
                return;
            }

            WikipediaBoundingBoxHelper _apihelper = new WikipediaBoundingBoxHelper(_username, 44.2, -10, -22.5, 55.3);
            
            string _apicall = _apihelper.TargetURL;

            System.Diagnostics.Debug.WriteLine("TargetURL=" + _apicall);

            m_msghelper.msg("invoking against " + _apicall);
            api_findNearbyPlaces.Invoke<APIMASH_WikiPediaLib.APIMASH_OM>(_apicall);
        }

        void api_findNearbyPlaces_OnResponse(object sender, APIMASHLib.APIMASHEvent e)
        {
            APIMASH_WikiPediaLib.APIMASH_OM _response = (APIMASH_WikiPediaLib.APIMASH_OM)e.Object;
            if (e.Status == APIMASHLib.APIMASHStatus.SUCCESS)
            {
                // copy data into bindable format for UI
                // not really using right now but it works
                _nearbyplaces.Copy(_response);

                if (_response.geonames == null)
                {
                    m_msghelper.msg("NO RESULTS RETURNED");
                    return;
                }

                m_msghelper.msg(string.Format("{0} geonames returned", _response.geonames.Count()));
                int _count = 1;
                foreach (APIMASH_WikiPediaLib.geoname gn in _response.geonames)
                {
                    m_msghelper.msg(string.Format("{0}:  {1}", _count.ToString(), gn.ToNearbyPlaceString()));
                    _count++;
                }
            }
            else
            {
                MessageDialogHelper.ShowMsg("oops", e.Message);
            }
        }
    }
}
