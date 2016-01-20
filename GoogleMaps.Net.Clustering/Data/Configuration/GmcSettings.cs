using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using GoogleMaps.Net.Clustering.Contract;

namespace GoogleMaps.Net.Clustering.Data.Configuration
{
    /// <summary>
    /// Configurations data
    /// Readonly fields
    /// Data are parsed from a config file
    /// </summary>
    public class GmcSettings : IGmcSettings
    {
        const string GoogleMapsSection = "googleMapsNetClustering";

        private static IGmcSettings _algoConfig;

        /// <summary>
        /// Singleton
        /// </summary>
        public static IGmcSettings Get
        {
            get
            {
                return _algoConfig ?? (_algoConfig = new GmcSettings());
            }
        }

        private GmcSettings()
        {
            var local = GetGoogleMapsSection();
            string s;

            GridX = int.Parse(local[s = "GridX"] ?? Throw(s));
            GridY = int.Parse(local[s = "GridY"] ?? Throw(s));
            DoShowGridLinesInGoogleMap = bool.Parse(local[s = "DoShowGridLinesInGoogleMap"] ?? Throw(s));
            OuterGridExtend = int.Parse(local[s = "OuterGridExtend"] ?? Throw(s));
            DoUpdateAllCentroidsToNearestContainingPoint = bool.Parse(local[s = "DoUpdateAllCentroidsToNearestContainingPoint"] ?? Throw(s));
            DoMergeGridIfCentroidsAreCloseToEachOther = bool.Parse(local[s = "DoMergeGridIfCentroidsAreCloseToEachOther"] ?? Throw(s));
            MergeWithin = double.Parse(local[s = "MergeWithin"] ?? Throw(s));
            MinClusterSize = int.Parse(local[s = "MinClusterSize"] ?? Throw(s));
            MaxMarkersReturned = int.Parse(local[s = "MaxMarkersReturned"] ?? Throw(s));
            AlwaysClusteringEnabledWhenZoomLevelLess = int.Parse(local[s = "AlwaysClusteringEnabledWhenZoomLevelLess"] ?? Throw(s));
            ZoomlevelClusterStop = int.Parse(local[s = "ZoomlevelClusterStop"] ?? Throw(s));
            MaxPointsInCache = int.Parse(local[s = "MaxPointsInCache"] ?? Throw(s));
            CacheServices = bool.Parse(local[s = "CacheServices"] ?? Throw(s));

            var types = (local[s = "MarkerTypes"] ?? Throw(s)).Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            MarkerTypes = new HashSet<int>();
            foreach (var type in types) MarkerTypes.Add(int.Parse(type));
        }

        // Use debug data
        public bool DoShowGridLinesInGoogleMap { get; private set; } // generate draw grid lines info to google map

        // How much data that is send to client
        // EDIT to extend to widen or shorten gridview for outside view, must be minimum 0
        // default value is 1 which returns same data as illustrated in the picture from my blog
        // (see googlemaps-clustering-viewport_ver1.png inside the Docements/Design folder)
        public int OuterGridExtend { get; private set; }

        // Move centroid point to nearest existing point?
        public bool DoUpdateAllCentroidsToNearestContainingPoint { get; private set; } 
        
        // Merge clusterpoints if close to each other?
        public bool DoMergeGridIfCentroidsAreCloseToEachOther { get; private set; } 
        
        // Cache get markers and get markers info services
        public bool CacheServices { get; private set; }

        // If neighbor cluster is within 1/n dist then merge, heuristic, higher value gives less merging
        public double MergeWithin { get; private set; }

        // Only cluster if minimum this number of points
        public int MinClusterSize { get; private set; }

        // If clustering is disabled, restrict number of markers returned
        public int MaxMarkersReturned { get; private set; }

        // Always cluster if equal or below this zoom level
        // to disable this effect set the value to -1
        public int AlwaysClusteringEnabledWhenZoomLevelLess { get; private set; }

        // Stop clustering from this zoom level and larger
        public int ZoomlevelClusterStop { get; private set; }

        // Grid array
        public int GridX { get; private set; }
        public int GridY { get; private set; }

        // Array of existing marker types
        public HashSet<int> MarkerTypes { get; private set; }

        // Max allowed points in memory cache
        public int MaxPointsInCache { get; private set; }
    
        public string Environment { get; private set; }

        public NameValueCollection GetGoogleMapsSection()
        {
            return ConfigurationManager.GetSection(GoogleMapsSection) as NameValueCollection;
        }

        static string Throw(string s)
        {
            throw new Exception(string.Format("GmcGlobalKeySettings setup error: {0}", s));
        }       
    }
}