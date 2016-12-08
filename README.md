# GoogleMaps.Net.Clustering
C# library for clustering map points. This library is very suitable for WebAPI (and similar-type) projects that don't depend on ASP.Net MVC.


![Clustering Img](https://raw.githubusercontent.com/pootzko/GoogleMaps.Net.Clustering/master/cluster-map.png "clustering image")

**Original Lib**  
This is a fork of [Google Maps Server-side Clustering with C#
](https://github.com/kunukn/Google-Maps-Clustering-CSharp) repo. The guys made a fast, working implementation of Google Maps clustering for C#. However, they tightly coupled it with MVC and WebForms where all I needed was a simple C# way to crunch a bunch of simple map points and convert them into cluster points.

**Installation**  
You can download the [GoogleMaps.Net.Clustering](https://www.nuget.org/packages/GoogleMaps.Net.Clustering/) package to install the latest version of GoogleMaps.Net.Clustering Lib.

Sponsored by [Dovetail Technologies](http://www.dovetail.ie/).

## Usage

This is an example of how to used cached clustering.

```cs
public IList<MapPoint> GetClusters(YourFilterObj filter)
{
    var clusterPointsCacheKey = "somecachekey";
    var points = GetClusterPointCollection(clusterPointsCacheKey);

    var mapService = new ClusterService(points);
    var input = new GetMarkersParams()
    {
        NorthEastLatitude = filter.NorthEastLatitude,
        NorthEastLongitude = filter.NorthEastLongitude,
        SouthWestLatitude = filter.SouthWestLatitude,
        SouthWestLongitude = filter.SouthWestLongitude,
        ZoomLevel = filter.ZoomLevel,
        PointType = clusterPointsCacheKey
    };

    var markers = mapService.GetClusterMarkers(input);

    return markers.Markers;
}

private PointCollection GetClusterPointCollection(string clusterPointsCacheKey)
{
    var points = new PointCollection();
    if (points.Exists(clusterPointsCacheKey))
        return points;

    var dbPoints = GetPoints(); // Get your points here
    var mapPoints = dbPoints.Select(p => new MapPoint() { X = p.X, Y = p.Y }).ToList();
    var cacheDuration = TimeSpan.FromHours(6);
    points.Set(mapPoints, cacheDuration, clusterPointsCacheKey);

    return points;
}
```

You will also need to add the following section to your `.conf` file (add it before the `connectionString` and/or `appSettings` section).

```xml
<googleMapsNetClustering>
    <!-- Configuration data for clustering, separate file to make the web.config file cleaner -->
    <!-- Read GmcSettings.cs for description -->
    <!-- debug purpose -->
    <add key="DoShowGridLinesInGoogleMap" value="false" />
    <!-- if bigger value then more expensive -->
    <add key="OuterGridExtend" value="0" />
    <!-- if true, then more expensive -->
    <add key="DoUpdateAllCentroidsToNearestContainingPoint" value="false" />
    <add key="DoMergeGridIfCentroidsAreCloseToEachOther" value="true" />
    <!-- cache get markers and get marker info services -->
    <add key="CacheServices" value="true" />
    <!--heuristic value for when to merge clusters when close to each other -->
    <add key="MergeWithin" value="2.9" />
    <add key="MinClusterSize" value="2" />
    <!-- restrict json length for client -->
    <add key="MaxMarkersReturned" value="500" />
    <!-- server-side control, set to -1 for disabled -->
    <add key="AlwaysClusteringEnabledWhenZoomLevelLess" value="2" />
    <add key="ZoomlevelClusterStop" value="15" />
    <add key="GridX" value="6" />
    <add key="GridY" value="5" />
    <add key="MarkerTypes" value="1;2;3" />
    <add key="MaxPointsInCache" value="100000000" />
</googleMapsNetClustering>
```
