
using System;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Xaml;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH
{
    public abstract class APIMASH_ApiBase : BindableBase
    {
        protected String _apiKey;

        private ApiMonitor _apiMonitor = Application.Current.Resources["ApiMonitor"] as ApiMonitor;
        public async Task<ApiResponse<T>> Invoke<T>(Func<String, T> deserializer, String uri, params object[] p)
        {
            ApiResponse<T> apiResponse = null;
            try
            {
                String expandedUri = String.Format(uri, p);
            
                var apiRequest = new ApiInvocation<T>(expandedUri);

                _apiMonitor.IsExecuting = true;
                apiResponse = await apiRequest.Invoke(deserializer);
                _apiMonitor.IsExecuting = false;
            }
            catch (Exception e)
            {
                apiResponse = new ApiResponse<T>(uri)
                    {
                        StatusCode = HttpStatusCode.Unused,
                        Message = e.Message,
                        Exception = e
                    };
            }
            finally
            {
                _apiMonitor.IsExecuting = false;
            }

            _apiMonitor.LastResponseStatus = apiResponse as ApiResponseStatus;
            return apiResponse;
        }

        public async Task<ApiResponse<T>> Invoke<T>(String uri, params object[] p)
        {
            return await Invoke<T>(null, uri, p);
        }
    }

    public class ApiMonitor : BindableBase
    {
        private ApiResponseStatus _apiResponseStatus;
        public ApiResponseStatus LastResponseStatus
        {
            get { return _apiResponseStatus; }
            set
            {
                SetProperty(ref _apiResponseStatus, value);
            }
        }

        private Boolean _isExecuting;
        public Boolean IsExecuting
        {
            get { return _isExecuting; }
            set
            {
                SetProperty(ref _isExecuting, value);
            }
        }

        private Boolean _debugEnabled = false;
        public Boolean DebugEnabled
        {
            get { return _debugEnabled; }
            set
            {
                SetProperty(ref _debugEnabled, value);
            }
        }
    }
}
