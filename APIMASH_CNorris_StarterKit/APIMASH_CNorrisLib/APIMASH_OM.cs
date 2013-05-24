using System.Runtime.Serialization;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_CNorrisLib
{
 /* sample payloads
  * 
  * { "type": "success", 
  *   "value": { "id": 268, 
  *              "joke": "Time waits for no man. Unless that man is John Doe." 
  *             } 
  * }
  * { "type": "success", 
  *   "value": { "id": 433, 
  *              "joke": "ITime waits for no man. Unless that man is John Doe", 
  *              "categories": [] } }
  */

    [DataContract]
    public class CNorrisJoke
    {
        [DataMember(Name="type")]
        public string Type { get; set; }

        [DataMember(Name="value")]
        public JokeValue Value { get; set; }
    }

    [DataContract]
    public class JokeValue
    {
        [DataMember(Name="id")]
        public string Id { get; set; }

        [DataMember(Name="joke")]
        public string Joke { get; set; }

        [DataMember(Name="caegories")]
        public string[] Categories { get; set; }
    }
}
