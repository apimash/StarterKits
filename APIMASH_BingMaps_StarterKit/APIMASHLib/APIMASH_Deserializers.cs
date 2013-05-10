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
    public static class Deserializers<T>
    {
        private static Dictionary<String, Func<String, T>> _mapping = new Dictionary<String, Func<String, T>>()
        {
            { "application/json", DeserializeJsonNet },
            { "application/xml", DeserializeXml }
        };

        public static Func<String, T> GetDefaultDeserializer(String mimeType)
        {
            if (_mapping.ContainsKey(mimeType))
                return _mapping[mimeType];
            else
                return null;
        }

        public static T DeserializeJsonNet(String objString)
        {
            try
            {
                return (T) JsonConvert.DeserializeObject<T>(objString);
            }
            catch (Exception) { throw; }
        }

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
