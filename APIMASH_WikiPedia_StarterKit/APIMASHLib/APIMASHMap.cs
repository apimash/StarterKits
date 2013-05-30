using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

///////////////////////////////////////////////////////////////////////////////////////////////
//
//  A P I   M A S H 
//
// This class makes the HTTP call and deserialzies the stream to a supplied Type
//
///////////////////////////////////////////////////////////////////////////////////////////////

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
            try
            {
                return (T)JsonConvert.DeserializeObject<T>(objString);
            }
            catch (Exception) 
            { 
                throw; 
            }
        }
#endif

        async public static Task<T> LoadObject<T>(string apiCall)
        {
            try
            {
                var ws = new HttpClient();
                var uriAPICall = new Uri(apiCall);
                var objString = await ws.GetStringAsync(uriAPICall);
                return (T)DeserializeObject<T>(objString);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
