using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH
{
    // contains outcome of HTTP along with any exception/message
    public class ApiResponseStatus : BindableBase
    {
        public String RequestUri { get; internal set; }
        public HttpStatusCode StatusCode { get; internal set; }
        public String Message { get; internal set; }
        public Exception Exception { get; internal set; }

        public Boolean CausedException { get { return this.Exception != null; } }
        public Boolean IsSuccessStatusCode { get { return ((Int32)this.StatusCode / 100U) == 2; } }

        internal ApiResponseStatus() {}
    }

    // contains the HTTP outcome along with the raw and deserialized responses
    public sealed class ApiResponse<T> : ApiResponseStatus
    {
        public String RawResponse { get; internal set; }
        public T DeserializedResponse { get; internal set; }

        public ApiResponse(String uri)
        {
            this.RequestUri = uri;
        }
    }

    // wrapper class for a given API call.  T is the type of the serialized response
    public class ApiInvocation<T>
    {
        public String Uri { get; private set; }
        public ApiInvocation(String uri)
        {
            Uri = uri;
        }

        async public Task<ApiResponse<T>> Invoke(Func<String, T> deserializer = null)
        {
            var apiResponse = new ApiResponse<T>(this.Uri);
            try
            {

                // more robust capture of exception for client request

                var httpClient = new HttpClient();
                var httpResponse = await httpClient.GetAsync(this.Uri);
                apiResponse.StatusCode = httpResponse.StatusCode;
                apiResponse.RawResponse = await httpResponse.Content.ReadAsStringAsync();

                if (apiResponse.IsSuccessStatusCode)
                {
                    var contentType = httpResponse.Content.Headers.ContentType.MediaType;
                    deserializer = deserializer ?? Deserializers<T>.GetDefaultDeserializer(contentType);

                    if (deserializer == null)
                        throw new Exception(String.Format("No deserializer specified for content type {0}", contentType));

                    apiResponse.DeserializedResponse = deserializer(apiResponse.RawResponse);
                }
            }
            catch (Exception e)
            {
                apiResponse.StatusCode = HttpStatusCode.Unused;
                apiResponse.Message = e.Message;
                apiResponse.Exception = e;
            }

            return apiResponse;
        }
    }
}
