using System;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH
{
    public static class Utilities
    {
        public static Double HaversineDistance(Double latitude1, Double longitude1, Double latitude2, Double longitude2)
        {
            Double R = 6372.8; // In kilometers
            Double dLat = toRadians(latitude2 - latitude1);
            Double dLon = toRadians(longitude2 - longitude1);

            Double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(toRadians(latitude1)) * Math.Cos(toRadians(latitude2));
            return R * 2 * Math.Asin(Math.Sqrt(a));
        }

        private static Double toRadians(Double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
