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

using APIMASH_WikiPedia_StarterKit.Common;

namespace APIMASH_WikiPedia_StarterKit
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class TextSearchPage : APIMASH_WikiPedia_StarterKit.Common.LayoutAwarePage
    {
        APIMASHLib.APIMASHInvoke m_api_wikipediaSearch;
        APIMASH_WikiPediaLib.APIMASH_geonamesCollection m_results;
        MsgHelper m_msghelper;

        public TextSearchPage()
        {
            this.InitializeComponent();
            m_api_wikipediaSearch = new APIMASHLib.APIMASHInvoke();
            m_api_wikipediaSearch.OnResponse += m_api_wikipediaSearch_OnResponse;
            m_results = new APIMASH_WikiPediaLib.APIMASH_geonamesCollection();

            m_msghelper = new MsgHelper(TextBlock_Msg);
            m_msghelper.clr();
            m_msghelper.msg("initialized and ready");
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
            string apicall = @"http://api.geonames.org/wikipediaSearchJSON?q=Tampa&maxRows=50&username=devfish";
            System.Diagnostics.Debug.WriteLine("OMG!  GET RID OF THIS HARD CODING OF THE URL!");
            m_msghelper.msg("invoking against " + apicall);
            m_api_wikipediaSearch.Invoke<APIMASH_WikiPediaLib.APIMASH_OM>(apicall);
        }

        void m_api_wikipediaSearch_OnResponse(object sender, APIMASHLib.APIMASHEvent e)
        {
            APIMASH_WikiPediaLib.APIMASH_OM _response = (APIMASH_WikiPediaLib.APIMASH_OM)e.Object;
            if (e.Status == APIMASHLib.APIMASHStatus.SUCCESS)
            {
                // copy data into bindable format for UI
                // not really using right now but it works
                m_results.Copy(_response);

                m_msghelper.msg(string.Format("{0} wikipedia text search matches returned", _response.geonames.Count()));
                int _count = 1;
                foreach (APIMASH_WikiPediaLib.geoname gn in _response.geonames)
                {
                    m_msghelper.msg(string.Format("{0}:  {1}", _count.ToString(), gn.ToWikipediaSearchResultString()));
                    _count++;
                }
            }
            else
            {
                APIMASH_WikiPedia_StarterKit.Common.MessageDialogHelper.ShowMsg("oops", e.Message);
            }
        }
    }
}




















