using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH
{
    /// <summary>
    /// Repository of deserializers to handle responses from API calls
    /// </summary>
    /// <typeparam name="T">Deserialized type, typically the type of the model class to store results for a given API call</typeparam>
    public static class Deserializers<T>
    {
        /// <summary>
        /// Map of Content-Type header values to deserializer classes 
        /// </summary>
        private static Dictionary<String, Func<Byte[], T>> _mapping = new Dictionary<String, Func<Byte[], T>>()
        {
            { "application/json", DeserializeJsonNet },
            { "application/xml", DeserializeXml },
            { "image/png", DeserializeImage },
            { "image/jpg", DeserializeImage },
            { "image/jpeg", DeserializeImage },
            { "image/gif", DeserializeImage }
        };

        /// <summary>
        /// Return default serializer for a given <paramref name="mediaType"/> (e.g., application/json)
        /// </summary>
        /// <param name="mediaType">mediaType of the response (from HTTP Content-Type header)</param>
        /// <returns>Function that accepts a byte stream and returns a serialize class of type T</returns>
        public static Func<Byte[], T> GetDefaultDeserializer(String mediaType)
        {
            if (_mapping.ContainsKey(mediaType))
                return _mapping[mediaType];
            else
                return null;
        }

        /// <summary>
        /// Deserialize payload via JSON.NET
        /// </summary>
        /// <param name="objBytes">Raw HTTP response byte array containing JSON</param>
        /// <returns>Deserialized object</returns>
        public static T DeserializeJsonNet(Byte[] objBytes)
        {
            try
            {
                return (T) JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(objBytes,0,objBytes.Length));
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Deserialize payload via .NET DataContractJsonSerializer
        /// </summary>
        /// <param name="objBytes">Raw HTTP response byte array containing JSON</param>
        /// <returns>Deserialized object</returns>
        public static T DeserializeJson(Byte[] objBytes)
        {
            using (var stream = new MemoryStream(objBytes))
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
        /// <param name="objBytes">Raw HTTP response byte array containing XML</param>
        /// <returns>Deserialized object</returns>
        public static T DeserializeXml(Byte[] objBytes)
        {
            using (var stream = new MemoryStream(objBytes))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T) serializer.Deserialize(stream);
                }
                catch (Exception e) { throw e; }
            }
        }

        /// <summary>
        /// Deserialize payload as Bitmap Image
        /// </summary>
        /// <param name="objBytes">Raw HTTP response byte array containing image bytes</param>
        /// <returns>BitmapImage</returns>
        public static T DeserializeImage(Byte[] objBytes)
        {
            try
            {
                BitmapImage image = new BitmapImage();

                // create a new in memory stream and datawriter
                using (var stream = new InMemoryRandomAccessStream())
                {
                    using (DataWriter dw = new DataWriter(stream))
                    {
                        // write the raw bytes and store synchronously
                        dw.WriteBytes(objBytes);
                        dw.StoreAsync().AsTask().Wait();
                        
                        // set the image source
                        stream.Seek(0);
                        image.SetSource(stream);
                    }
                }

                return (T) ((object) image);
            }
            catch (Exception e) { throw e; }
        }
    }
}
