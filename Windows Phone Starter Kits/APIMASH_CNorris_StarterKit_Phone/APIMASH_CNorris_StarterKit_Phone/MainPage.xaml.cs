using System.Windows;
using Microsoft.Phone.Controls;
using APIMASHLib;
using APIMASH_CNorrisLib;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_CNorris_StareterKit_Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        readonly APIMASHInvoke apiInvoke;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            apiInvoke = new APIMASHInvoke();
            apiInvoke.OnResponse += apiInvoke_OnResponse;
            Invoke();
        }

        private void Invoke()
        {
            const string apiCall = @"http://api.icndb.com/jokes/random?exclude=[explicit]";
            apiInvoke.Invoke<CNorrisJoke>(apiCall);
        }

        async private void apiInvoke_OnResponse(object sender, APIMASHEvent e)
        {
            var response = (CNorrisJoke)e.Object;

            if (e.Status == APIMASHStatus.SUCCESS)
            {
                var s = response.Value.Joke;
                s = s.Replace("&quot;", "'");
                Joke.Text = s;
            }
            else
            {
                MessageBox.Show(e.Message);
            }
        }

        private void HitMeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Invoke();
        }
    }
}