using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
    public static class APIMASHMap
    {
#if OBJMAP_JSON // .NET JSON
        public static string SerializeObject(object objectToSerialize)
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    var serializer = new DataContractJsonSerializer(objectToSerialize.GetType());
                    serializer.WriteObject(stream, objectToSerialize);
                    stream.Position = 0;
                    var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Serialize:" + e.Message);
                    return string.Empty;
                }
            }
        }

        public static T DeserializeObject<T>(string objString)
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(objString)))
            {
                try
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(stream);
                }
                catch (Exception) { throw; }
            }
        }
#endif

#if OBJMAP_XML // .NET XML
        public static T DeserializeObject<T>(string objString)
        {
            using (MemoryStream _Stream = new MemoryStream(Encoding.Unicode.GetBytes(objString)))
            {
                try
                {

                    DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)_serializer.ReadObject(_Stream);

                    XmlSerializer _serializer = new XmlSerializer(typeof(T));
                    return (T)_serializer.Deserialize(_Stream);
                }
                catch (Exception) { throw; }
            }
        }
#endif

#if OBJMAP_NEWTONSOFT // JSON.NET
        public static T DeserializeObject<T>(string objString)
        {
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            return (T)JsonConvert.DeserializeObject<T>(objString, settings);
        }
#endif

        private static string DecompressBytes(byte[] compressedBytes)
        {
            string uncompressedObj = string.Empty;
            // http://stackoverflow.com/questions/12894406/inflating-a-compressed-byte-array-in-winrt
            // The reason for skipping the first two bytes is that they are part of the zlib spec and not the deflate spec: 
            // http://george.chiramattel.com/blog/2007/09/deflatestream-block-length-does-not-match.html
            using (System.IO.Compression.GZipStream stream = new System.IO.Compression.GZipStream(new MemoryStream(compressedBytes, 0, compressedBytes.Length), System.IO.Compression.CompressionMode.Decompress))
            {
                MemoryStream memory = new MemoryStream();
                byte[] writeData = new byte[4096];
                int resLen;
                while ((resLen = stream.Read(writeData, 0, writeData.Length)) > 0)
                {
                    memory.Write(writeData, 0, resLen);
                }
                var uncompressedBytes = memory.ToArray();
                uncompressedObj = Encoding.UTF8.GetString(uncompressedBytes, 0, uncompressedBytes.Length);
            }
            return uncompressedObj;
        }

        async public static Task<T> LoadObject<T>(string apiCall)
        {
            var ws = new HttpClient();
            var uriAPICall = new Uri(apiCall);
            var objString = await ws.GetStringAsync(uriAPICall);
            return (T)DeserializeObject<T>(objString);
        }

        async public static Task<T> LoadCompressedObject<T>(string apiCall)
        {
            var ws = new HttpClient();
            ws.DefaultRequestHeaders.AcceptEncoding.Add( StringWithQualityHeaderValue.Parse("DEFLATE") );
            var uriAPICall = new Uri(apiCall);
            var objArray = await ws.GetByteArrayAsync(uriAPICall);
            var uncompressedString = DecompressBytes(objArray);
            return (T)DeserializeObject<T>(uncompressedString);
        }
    }
}
