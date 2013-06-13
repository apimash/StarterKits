using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
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
        public HttpStatusCode StatusCode {get; internal set;}

        /// <summary>
        /// Explanatory message (or <paramref name="Exception"/> message if one occurred)
        /// </summary>
        public String Message { get; internal set; }

        /// <summary>
        /// Exception object, if one occured
        /// </summary>
        public Exception Exception {get; internal set;}

        /// <summary>
        /// Shortcut to detecting success return code (200 - 299)
        /// </summary>
        public Boolean IsSuccessStatusCode { get { return (this.StatusCode == 0) || ((Int32)this.StatusCode / 100U) == 2; } }

        /// <summary>
        /// Default instance representing an unexecuted request
        /// </summary>
        public static ApiResponseStatus Default { 
            get { return new ApiResponseStatus() { StatusCode = 0 }; }
        }        

        /// <summary>
        /// Sets custom status message and (optionally) code
        /// </summary>
        /// <param name="message">Error or status message</param>
        /// <param name="statusCode">Optional HTTP Status code (defaults to 306 "Unused")</param>
        public void SetCustomStatus(String message, HttpStatusCode statusCode = HttpStatusCode.Unused)
        {
            this.Message = message;
            this.StatusCode = StatusCode;
        }
        
        internal ApiResponseStatus() {}
    }

    /// <summary>
    /// Container for HTTP raw and deserialized responses in addition to status information resulting from API call
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
        public Byte[] RawResponse { get; internal set; }

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
    internal class ApiInvocation<T>
    {
        /// <summary>
        /// URI endpoint of the API call
        /// </summary>
        internal String Uri { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uri">URI endpoint of the API call</param>
        internal ApiInvocation(String uri)
        {
            Uri = uri;
        }

        /// <summary>
        /// Invoke API call on URI provided when class was initialized
        /// </summary>
        /// <param name="deserializer">Deserializer method to use on the HTTP response payload. If null, a default deserializer will be selected based on the mediaType found in the HTTP response Content-Type header.</param>
        /// <returns>An <see cref="ApiResponse"/> containing the HTTP response payload, headers, and status or error information</returns>
        async internal Task<ApiResponse<T>> Invoke(Func<Byte[], T> deserializer = null)
        {

            // set up a new response class to capture the results
            var apiResponse = new ApiResponse<T>(this.Uri);
            try
            {
                // invoke the GET request
                var httpClient = new HttpClient();
                var httpResponse = await httpClient.GetAsync(this.Uri);

                // capture the status code, headers, and raw response
                apiResponse.StatusCode = httpResponse.StatusCode;
                apiResponse.Headers = httpResponse.Headers;
                apiResponse.RawResponse = await httpResponse.Content.ReadAsByteArrayAsync();

                // if successful
                if (apiResponse.IsSuccessStatusCode)
                {
                    // determine contentType and select appropriate deserializer
                    var contentType = httpResponse.Content.Headers.ContentType;
                    var mediaType = contentType != null ? contentType.MediaType : String.Empty;

                    deserializer = deserializer ?? Deserializers<T>.GetDefaultDeserializer(mediaType);

                    if (deserializer == null)
                        throw new Exception(String.Format("No deserializer specified for content type {0}", mediaType));

                    // deserialize response
                    apiResponse.DeserializedResponse = deserializer(apiResponse.RawResponse);
                }
            }
            catch (Exception e)
            {
                // capture exception that may have been raised and store the response class
                apiResponse.StatusCode = HttpStatusCode.Unused;

                // get to the root exception message
                Exception ne;
                for (ne = e; ne.InnerException != null; ne = ne.InnerException)
                    ;
                apiResponse.Message = ne.Message;
                apiResponse.Exception = ne;
            }

            return apiResponse;
        }
    }
}
