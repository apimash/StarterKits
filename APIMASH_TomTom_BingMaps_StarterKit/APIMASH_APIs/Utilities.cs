using System;

namespace APIMASH
{
    public static class Utilities
    {
        public static double HaversineDistance(Double latitiude1, Double longitude1, Double latitude2, Double longitude2)
        {
            var R = 6372.8; // In kilometers
            var dLat = toRadians(latitude2 - latitiude1);
            var dLon = toRadians(longitude2 - longitude1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(toRadians(longitude1)) * Math.Cos(toRadians(longitude2));
            return R * 2 * Math.Asin(Math.Sqrt(a));
        }

        private static double toRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
