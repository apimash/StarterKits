using System;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH.Mapping
{
    /// <summary>
    /// Bounding box of latitude/longitude
    /// </summary>
    public sealed class BoundingBox
    {
        /// <summary>
        /// Northernmost latitude value
        /// </summary>
        public readonly Double North;

        /// <summary>
        /// Southernmost latitude value
        /// </summary>
        public readonly Double South;

        /// <summary>
        /// Westernmost longitude value
        /// </summary>
        public readonly Double West;  
      
        /// <summary>
        /// Easternmost longitude value
        /// </summary>
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
    public sealed class LatLong
    {
        /// <summary>
        /// Latitude component
        /// </summary>
        public readonly Double Latitude;

        /// <summary>
        /// Longitude component
        /// </summary>
        public readonly Double Longitude;

        public LatLong(Double latitude, Double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
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

        /// <summary>
        /// Calculate a Bing Maps zoom level that will fully incorporate the "bounding box" rectangle of
        /// a location search in the tile size alloted.
        /// </summary>
        /// <param name="latitude">Latitude of center point of extent</param>
        /// <param name="extent">Number of meters of longest side of bounding box for given location</param>
        /// <param name="dimension">Size (in pixels) of desired square map image</param>
        /// <returns></returns>
        public static Int32 OptimalZoomLevel(Double latitude, Double extent, Int32 sizeInPixels)
        {
            // determine the resolution (meters/pixel) needed based on the size of the bounding box returned from
            // the location query.
            Double resolution = extent / (double) sizeInPixels;

            // scary math (see: http://msdn.microsoft.com/en-us/library/aa940990.aspx)
            var zoomLevel = (Int32)Math.Floor(Math.Log(156543.04 * Math.Cos(toRadians(latitude)) / resolution, 2));

            // make sure the result doesn't get to far out of whack
            return Math.Max(1, Math.Min(zoomLevel, 16));
        }

        private static Double toRadians(Double angle)
        {
            return Math.PI * angle / 180.0;
        }    
    }
}
