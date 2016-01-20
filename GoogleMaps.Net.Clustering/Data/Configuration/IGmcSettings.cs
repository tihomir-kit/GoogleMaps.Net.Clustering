using System.Collections.Generic;

namespace GoogleMaps.Net.Clustering.Data.Configuration
{
    public interface IGmcSettings
    {
        bool DoShowGridLinesInGoogleMap { get; }

        int OuterGridExtend { get; }

        bool DoUpdateAllCentroidsToNearestContainingPoint { get; }

        bool DoMergeGridIfCentroidsAreCloseToEachOther { get; }

        double MergeWithin { get; }

        int MinClusterSize { get; }

        int MaxMarkersReturned { get; }

        int AlwaysClusteringEnabledWhenZoomLevelLess { get; }

        int ZoomlevelClusterStop { get; }

        int GridX { get; }

        int GridY { get; }

        HashSet<int> MarkerTypes { get; }

        int MaxPointsInCache { get; }

        string Environment { get; }

        bool CacheServices { get; }
    }
}
