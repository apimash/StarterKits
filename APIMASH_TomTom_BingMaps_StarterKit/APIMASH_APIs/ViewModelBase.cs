using APIMASH;
using System;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_APIs
{
    /// <summary>
    /// Basic class to use as view model for user interface elements exposing resuls of a given, single API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiViewModelBase<T> : APIMASH.BindableBase where T : APIMASH.ApiBase, new()
    {
        /// <summary>
        /// API wrapper class (extends APIMASH.ApiBase) and provides access to API calls and results
        /// </summary>
        public T ApiClass
        {
            get { return _apiClass; }
            set { SetProperty(ref _apiClass, value); }
        }
        private T _apiClass;

        /// <summary>
        /// API response status
        /// </summary>
        public ApiResponseStatus ApiStatus
        {
            get { return _apiStatus; }
            set { SetProperty (ref _apiStatus, value);
                  this.Refreshed = true;
                }
        }
        private ApiResponseStatus _apiStatus;

        /// <summary>
        /// Sentinel value to determine if a request has been made since view model was instantiated
        /// </summary>
        public Boolean Refreshed
        {
            get { return _refreshed; }
            set { SetProperty(ref _refreshed, value); }
        }
        private Boolean _refreshed;

        public ApiViewModelBase()
        {
            ApiClass = new T();
            ApiStatus = ApiResponseStatus.DefaultInstance;
            Refreshed = false;
        }
    }
}
