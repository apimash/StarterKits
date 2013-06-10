using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

/*
 *
 *  A P I   M A S H 
 *
 * This class makes the HTTP call and deserialzies the stream to a supplied Type
*/

namespace APIMASHLib
{
    public class APIMASHMap : WebClient
    {
        public static T DeserializeObject<T>(string objString)
        {
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            return (T) JsonConvert.DeserializeObject<T>(objString, settings);
        }

        public async Task<T> LoadObject<T>(string apiCall)
        {
            var uriAPICall = new Uri(apiCall);
            var objString = await GetStringAsync(uriAPICall);
            return DeserializeObject<T>(objString);
        }

        public Task<string> GetStringAsync(Uri requestUri)
        {
            var tcs = new TaskCompletionSource<string>();

            try
            {
                DownloadStringCompleted += (s, e) =>
                {
                    if (e.Error == null)
                    {
                        tcs.TrySetResult(e.Result);
                    }
                    else
                    {
                        tcs.TrySetException(e.Error);
                    }
                };

                this.DownloadStringAsync(requestUri);

            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            if (tcs.Task.Exception != null)
            {
                throw tcs.Task.Exception;
            }

            return tcs.Task;
        }
    }
}
