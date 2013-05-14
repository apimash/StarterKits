using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH
{
    /// <summary>
    /// Repository of deserializers to handle response from API calls
    /// </summary>
    /// <typeparam name="T">Deserialized type, typically the type of the model class to store results for a given API call</typeparam>
    public static partial class Deserializers<T>
    {
        /// <summary>
        /// Map of Content-Type header values to deserializer classes 
        /// </summary>
        private static Dictionary<String, Func<String, T>> _mapping = new Dictionary<String, Func<String, T>>()
        {
            { "application/json", DeserializeJsonNet },
            { "application/xml", DeserializeXml }
        };

        /// <summary>
        /// Return default serializer for a given <paramref name="mediaType"/> (e.g., application/json)
        /// </summary>
        /// <param name="mediaType">mediaType of the response (from HTTP Content-Type header)</param>
        /// <returns></returns>
        public static Func<String, T> GetDefaultDeserializer(String mediaType)
        {
            if (_mapping.ContainsKey(mediaType))
                return _mapping[mediaType];
            else
                return null;
        }

        /// <summary>
        /// Deserialize payload via JSON.NET
        /// </summary>
        /// <param name="objString">Raw HTTP response string containing JSON</param>
        /// <returns>Deserialized object</returns>
        public static T DeserializeJsonNet(String objString)
        {
            try
            {
                return (T) JsonConvert.DeserializeObject<T>(objString);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Deserialize payload via .NET DataContractJsonSerializer
        /// </summary>
        /// <param name="objString">Raw HTTP response string containing JSON</param>
        /// <returns>Deserialized object</returns>
        public static T DeserializeJson(String objString)
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

        /// <summary>
        /// Deserialize payload via .NET XmlSerializer
        /// </summary>
        /// <param name="objString">Raw HTTP response string containing XML</param>
        /// <returns>Deserialized object</returns>
        public static T DeserializeXml(string objString)
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(objString)))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T) serializer.Deserialize(stream);
                }
                catch (Exception e) { throw e; }
            }
        }
    }
}
