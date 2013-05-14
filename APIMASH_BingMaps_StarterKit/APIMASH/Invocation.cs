using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH
{
    /// <summary>
    /// Status information resulting from an executed API call
    /// </summary>
    public class ApiResponseStatus
    {
        /// <summary>
        /// URI requested
        /// </summary>
        public String RequestUri {get; internal set;}

        /// <summary>
        /// HTTP Status Code (Unused 306 used to mark non HTTP errors, typically exceptions)
        /// </summary>
        public HttpStatusCode StatusCode {get; set;}

        /// <summary>
        /// Explanatory message (or <paramref name="Exception"/> message if one occurred)
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// Exception object, if one occured
        /// </summary>
        public Exception Exception {get; internal set;}

        /// <summary>
        /// Shortcut to detecting success return code (200 - 299)
        /// </summary>
         public Boolean IsSuccessStatusCode { get { return ((Int32)this.StatusCode / 100U) == 2; } }

        /// <summary>
        /// Return a default instance representing an unexecuted request
        /// </summary>
        public static ApiResponseStatus DefaultInstance { 
            get { return new ApiResponseStatus() { StatusCode = HttpStatusCode.OK }; }
        }        
        
        internal ApiResponseStatus() {}
    }

    /// <summary>
    /// HTTP raw and deserialized responses in addition to status information resulting from API call
    /// </summary>
    /// <typeparam name="T">The deserialized data type of results from the API call (the type of the model class)</typeparam>
    public sealed class ApiResponse<T> : ApiResponseStatus
    {
        /// <summary>
        /// HTTP response headers
        /// </summary>
        public HttpResponseHeaders Headers { get; internal set; }

        /// <summary>
        /// HTTP response payload
        /// </summary>
        public String RawResponse { get; internal set; }

        /// <summary>
        /// Payload deserialized into a class of <paramref name="T"/>, the model class type
        /// </summary>
        public T DeserializedResponse { get; internal set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uri">URI endpoint for the API call that resulted in the current response</param>
        public ApiResponse(String uri)
        {
            this.RequestUri = uri;
        }
    }

    /// <summary>
    /// Class representing a single invocation of an API
    /// </summary>
    /// <typeparam name="T">The deserialized data type of results from the API call (the type of the model class)</typeparam>
    public class ApiInvocation<T>
    {
        /// <summary>
        /// URI endpoint of the API call
        /// </summary>
        public String Uri { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uri">URI endpoint of the API call</param>
        public ApiInvocation(String uri)
        {
            Uri = uri;
        }

        /// <summary>
        /// Invoke API call on URI provided when class was initialized
        /// </summary>
        /// <param name="deserializer">Deserializer method to use on the HTTP response payload. If null, a default deserializer will be selected based on the mediaType found in the HTTP response Content-Type header.</param>
        /// <returns>An <see cref="ApiResponse"/> containing the HTTP response payload, headers, and status or error information</returns>
        async public Task<ApiResponse<T>> Invoke(Func<String, T> deserializer = null)
        {

            // set up a new response class to capture the results
            var apiResponse = new ApiResponse<T>(this.Uri);
            try
            {
                // invoke the Get request
                var httpClient = new HttpClient();
                var httpResponse = await httpClient.GetAsync(this.Uri);

                // capture the status code, headers, and raw response
                apiResponse.StatusCode = httpResponse.StatusCode;
                apiResponse.Headers = httpResponse.Headers;                
                apiResponse.RawResponse = await httpResponse.Content.ReadAsStringAsync();

                // if successful
                if (apiResponse.IsSuccessStatusCode)
                {
                    // determine contentType and select appropriate deserializer
                    var contentType = httpResponse.Content.Headers.ContentType.MediaType;
                    deserializer = deserializer ?? Deserializers<T>.GetDefaultDeserializer(contentType);

                    if (deserializer == null)
                        throw new Exception(String.Format("No deserializer specified for content type {0}", contentType));

                    // deserialize response
                    apiResponse.DeserializedResponse = deserializer(apiResponse.RawResponse);
                }
            }
            catch (Exception e)
            {
                // capture exceptions that may have been raised and store the response class
                apiResponse.StatusCode = HttpStatusCode.Unused;
                apiResponse.Message = e.Message;
                apiResponse.Exception = e;
            }

            return apiResponse;
        }
    }
}
