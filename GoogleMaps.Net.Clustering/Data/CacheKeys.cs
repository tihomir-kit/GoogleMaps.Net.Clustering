using System;

namespace GoogleMaps.Net.Clustering.Data
{
    internal static class CacheKeys
    {
        private const string gmcKN = "gmcKN";

        public static string PointsDatabase = String.Concat(gmcKN, "PointsDatabase");

        public static string GetMarkers(int i)
        {
            return String.Concat(gmcKN, "GetMarkers", i);
        }

        public static string GetMarkerInfo(int i)
        {
            return String.Concat(gmcKN, "GetMarkerInfo", i);
        }
    }
}
