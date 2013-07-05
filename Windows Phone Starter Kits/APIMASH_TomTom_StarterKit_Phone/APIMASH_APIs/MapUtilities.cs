using System;
using System.Runtime.Serialization;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH.Mapping
{
    /// <summary>
    /// Bounding box of latitude/longitude
    /// </summary>
    ///             
    [DataContract]
    public sealed class BoundingBox
    {
        /// <summary>
        /// Northernmost latitude value
        /// </summary>
        [DataMember]
        public readonly Double North;

        /// <summary>
        /// Southernmost latitude value
        /// </summary>
        [DataMember]
        public readonly Double South;

        /// <summary>
        /// Westernmost longitude value
        /// </summary>
        [DataMember]
        public readonly Double West;  
      
        /// <summary>
        /// Easternmost longitude value
        /// </summary>
        [DataMember]
        public readonly Double East;

        /// <summary>
        /// Create a new bounding box object
        /// </summary>
        /// <param name="n">Northernmost latitudinal point</param>
        /// <param name="s">Southernmost latitudinal point</param>
        /// <param name="w">Westernmost longitudinal point</param>
        /// <param name="e">Easternmost longitudinal point</param>
        public BoundingBox(Double n, Double s, Double w, Double e)
        {
            North = n;
            South = s;
            East = e;
            West = w;
        }
    }
    
    /// <summary>
    /// Latitude/longitude structure
    /// </summary>
    [DataContract]
    public sealed class LatLong
    {
        /// <summary>
        /// Latitude component
        /// </summary>
        [DataMember]
        public readonly Double Latitude;

        /// <summary>
        /// Longitude component
        /// </summary>
        [DataMember]
        public readonly Double Longitude;

        /// <summary>
        /// Creates a new lat/long structure
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        public LatLong(Double latitude, Double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Parses a pipe-delimited lat/long string into a class instance
        /// </summary>
        /// <param name="encodedString">String of format latitude|longitude</param>
        /// <param name="result">Resulting LatLong instance</param>
        /// <returns>True if input is valid format</returns>
        public static Boolean TryParse(String encodedString, out LatLong result)
        {
            String  latPartString;
            String  longPartString;
            double  latPartDouble;
            double  longPartDouble;

            // check for delimter
            var pipePos = encodedString.IndexOf('|');
            if (pipePos >= 0)
            {
                // extract string components
                latPartString = encodedString.Substring(0, pipePos);
                longPartString = encodedString.Substring(pipePos + 1);

                // try parsing each into doubles
                if (Double.TryParse(latPartString, out latPartDouble) && Double.TryParse(longPartString, out longPartDouble))
                {
                    result = new LatLong(latPartDouble, longPartDouble);
                    return true;
                } 
            }

            // failed to parse
            result = null;
            return false;
        }

        /// <summary>
        /// String representation of lat/long pair
        /// </summary>
        /// <returns>Pipe delimited string of latitude and longitude</returns>
        public override string ToString()
        {
            return String.Format("{0}|{1}", this.Latitude, this.Longitude);
        }
    }

    /// <summary>
    /// Map-focused utility methods
    /// </summary>
    public static class MapUtilities
    {
        /// <summary>
        /// Calculate distance (in meters) between two points on the earth using the Haversine formula
        /// </summary>
        /// <param name="point1">First location as latitude/longitude pair</param>
        /// <param name="point2">Second location as latitude/longitude pair</param>
        /// <returns></returns>
        public static Double HaversineDistance(LatLong point1, LatLong point2)
        {
            Double R = 6371009;
            Double dLat = toRadians(point2.Latitude - point1.Latitude);
            Double dLon = toRadians(point2.Longitude - point1.Longitude);

            Double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * 
                Math.Cos(toRadians(point1.Latitude)) * Math.Cos(toRadians(point2.Latitude));
            return R * 2 * Math.Asin(Math.Sqrt(a));
        }

        private static Double toRadians(Double angle)
        {
            return Math.PI * angle / 180.0;
        }    
    }
}
