using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH
{
    /// <summary>
    /// Base class for all API wrapper classes
    /// </summary>
    public abstract class ApiBase : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged handling
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion  

        /// <summary>
        /// API access key (required by most APIs)
        /// </summary>
        protected String _apiKey;
        
        /// <summary>
        /// Call API specified via <paramref name="uri"/>
        /// </summary>
        /// <typeparam name="T">Type of data returned from API call following deserialization. This is typically the type of the model associated with the given API call.</typeparam>
        /// <param name="deserializer">Deserializer method used to convert the raw HTTP response to the model type <typeparamref name="T"/></param>
        /// <param name="uri">Full URI of the API call with optional format items</param>
        /// <param name="p">Optional parameters to be supplied to the format items in the <paramref name="uri"/></param>
        /// <returns><paramref name="ApiResponse&lt;T&gt;"/> where <typeparamref name="T"/> is the deserialzed (model) data type.</returns>
        public async Task<ApiResponse<T>> Invoke<T>(Func<String, T> deserializer, String uri, params object[] p)
        {
            // access API Monitor resource (useful but not required)
            ApiMonitor _apiMonitor = null;
            if (Application.Current.Resources.ContainsKey("ApiMonitor"))
                _apiMonitor = Application.Current.Resources["ApiMonitor"] as ApiMonitor;

            ApiResponse<T> apiResponse = null;
            try
            {
                // expand any parameters in the URI string
                String expandedUri = String.Format(uri, p);
            
                // set up the API call
                var apiRequest = new ApiInvocation<T>(expandedUri);
                if (_apiMonitor != null) _apiMonitor.IsExecuting = true;

                // invoke asynchronously, if API call failed or caused exception, that information is recorded in apiResponse
                apiResponse = await apiRequest.Invoke(deserializer);
            }
            catch (Exception e)
            {
                // capture non-HTTP exception that may have occurred prior to the API invocation
                apiResponse = new ApiResponse<T>(uri)
                    {
                        StatusCode = HttpStatusCode.Unused,
                        Message = e.Message,
                        Exception = e
                    };
            }
            finally
            {
                if (_apiMonitor != null) _apiMonitor.IsExecuting = false;
            }

            // record outcome of API call in ApiMonitor for visibility at application level
            if (_apiMonitor != null) _apiMonitor.LastResponseStatus = apiResponse as ApiResponseStatus;

            // return the API response, including raw and deserialized content
            return apiResponse;
        }

        /// <summary>
        /// Call API specified via <paramref name="uri"/>
        /// </summary>
        /// <typeparam name="T">Type of data returned from API call following deserialization. This is typically the type of the 'model' associated with the given API call. A default deserializer will be selected based on the Content-Type of the HTTP response.</typeparam>
        /// <param name="uri">Full URI of the API call with optional format items</param>
        /// <param name="p">Optional parameters to be supplied to the format items in the <paramref name="uri"/></param>
        /// <returns><paramref name="ApiResponse&lt;T&gt;"/> where <typeparamref name="T"/> is the deserialzed (model) data type.</returns>
        public async Task<ApiResponse<T>> Invoke<T>(String uri, params object[] p)
        {
            return await Invoke<T>(null, uri, p);
        }
    }

    /// <summary>
    /// Class used to track status of API invocations across the entire application. 
    /// <para>This class should be a singleton and included as a resource in the App.xaml file
    /// <code><apimash:ApiMonitor x:Key="ApiMonitor" /></para>
    /// <para>This class does not synchronize across simultaneous API calls.</para>
    /// </summary>
    public class ApiMonitor : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged handling
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion  

        /// <summary>
        /// HTTP status code (or exception details) from last executed API call
        /// </summary>
        public ApiResponseStatus LastResponseStatus
        {
            get { return _lastResponseStatus; }
            set { SetProperty(ref _lastResponseStatus, value); }
        }
        private ApiResponseStatus _lastResponseStatus;

        /// <summary>
        /// Indicator whether an API call is in currently executing (useful for databinding a progress indicator)
        /// </summary>
        public Boolean IsExecuting
        {
            get { return _isExecuting; }
            set { SetProperty(ref _isExecuting, value); }
        }
        private Boolean _isExecuting;
    }
}