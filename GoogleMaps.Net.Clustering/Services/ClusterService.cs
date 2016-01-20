using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GoogleMaps.Net.Clustering.Algorithm;
using GoogleMaps.Net.Clustering.Contract;
using GoogleMaps.Net.Clustering.Data;
using GoogleMaps.Net.Clustering.Data.Configuration;
using GoogleMaps.Net.Clustering.Data.Geometry;
using GoogleMaps.Net.Clustering.Data.Params;
using GoogleMaps.Net.Clustering.Data.Responses;
using GoogleMaps.Net.Clustering.Extensions;
using GoogleMaps.Net.Clustering.Infrastructure;
using GoogleMaps.Net.Clustering.Services;
using GoogleMaps.Net.Clustering.Utility;

namespace GoogleMaps.Net.Clustering.Services
{
    public class ClusterService : IClusterService
    {
        private readonly IPointCollection _pointCollection;
        private readonly IMemCache _memCache;

        public ClusterService(IPointCollection pointCollection)
        {
            _pointCollection = pointCollection;
            _memCache = new MemCache();
        }

        public ClusterMarkersResponse GetClusterMarkers(GetMarkersParams getParams)
        {
            // Decorate with elapsed time
            var sw = new Stopwatch();
            sw.Start();
            var reply = GetMarkersHelper(getParams);
            sw.Stop();
            reply.Elapsed = sw.Elapsed.ToString();
            return reply;
        }

        /// <summary>
        /// Read Through Cache
        /// </summary>
        /// <param name="getParams"></param>
        /// <returns></returns>
        private ClusterMarkersResponse GetMarkersHelper(GetMarkersParams getParams)
        {
            try
            {
                var neLat = getParams.NorthEastLatitude;
                var neLong = getParams.NorthEastLongitude;
                var swLat = getParams.SouthWestLatitude; 
                var swLong = getParams.SouthWestLongitude;
                var zoomLevel = getParams.ZoomLevel;
                var filter = getParams.Filter ?? "";

                // values are validated there
                var markersInput = new MarkersInput(neLat, neLong, swLat, swLong, zoomLevel, filter);

                var grid = GridCluster.GetBoundaryExtended(markersInput);
                var cacheKeyHelper = string.Format("{0}_{1}_{2}", markersInput.Zoomlevel, markersInput.FilterHashCode(), grid.GetHashCode());
                var cacheKey = CacheKeys.GetMarkers(cacheKeyHelper.GetHashCode());

                var response = new ClusterMarkersResponse();

                markersInput.Viewport.ValidateLatLon(); // Validate google map viewport input (should be always valid)
                markersInput.Viewport.Normalize();

                // Get all points from memory
                IList<MapPoint> points = _pointCollection.Get(getParams.PointType); // _pointsDatabase.GetPoints();

                // Filter points
                points = FilterUtil.FilterByType(
                    points,
                    new FilterData { TypeFilterExclude = markersInput.TypeFilterExclude }
                );



                // Create new instance for every ajax request with input all points and json data
                var clusterAlgo = new GridCluster(points, markersInput);

                var clusteringEnabled = markersInput.IsClusteringEnabled
                    || GmcSettings.Get.AlwaysClusteringEnabledWhenZoomLevelLess > markersInput.Zoomlevel;

                // Clustering
                if (clusteringEnabled && markersInput.Zoomlevel < GmcSettings.Get.ZoomlevelClusterStop)
                {
                    IList<MapPoint> markers = clusterAlgo.RunCluster();

                    response = new ClusterMarkersResponse
                    {
                        Markers = markers,
                        Polylines = clusterAlgo.GetPolyLines()
                    };
                }
                else
                {
                    // If we are here then there are no clustering
                    // The number of items returned is restricted to avoid json data overflow
                    IList<MapPoint> filteredDataset = FilterUtil.FilterDataByViewport(points, markersInput.Viewport);
                    IList<MapPoint> filteredDatasetMaxPoints = filteredDataset.Take(GmcSettings.Get.MaxMarkersReturned).ToList();

                    response = new ClusterMarkersResponse
                    {
                        Markers = filteredDatasetMaxPoints,
                        Polylines = clusterAlgo.GetPolyLines(),
                        Mia = filteredDataset.Count - filteredDatasetMaxPoints.Count,
                    };
                }
                
                // if client ne and sw is inside a specific grid box then cache the grid box and the result
                // next time test if ne and sw is inside the grid box and return the cached result
                if (GmcSettings.Get.CacheServices)
                    _memCache.Set(cacheKey, response, TimeSpan.FromMinutes(10)); // cache data

                return response;
            }
            catch (Exception ex)
            {
                return new ClusterMarkersResponse
                {
                    OperationResult = "0",
                    ErrorMessage = string.Format("MapService says: exception {0}", ex.Message)
                };
            }
        }


        public MarkerInfoResponse GetMarkerInfo(int uid, string pointType = null)
        {
            // Decorate with elapsed time
            var sw = new Stopwatch();
            sw.Start();
            var reply = GetMarkerInfoResponse(uid);
            sw.Stop();
            reply.Elapsed = sw.Elapsed.ToString();
            return reply;
        }


        /// <summary>
        /// Read Through Cache
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private MarkerInfoResponse GetMarkerInfoResponse(int uid, string pointType = null)
        {
            try
            {
                var cacheKey = CacheKeys.GetMarkerInfo(uid);
                var reply = _memCache.Get<MarkerInfoResponse>(cacheKey);
                if (reply != null)
                {
                    // return cached data
                    reply.IsCached = true;
                    return reply;
                }

                MapPoint marker = _pointCollection.Get(pointType).SingleOrDefault(i => i.MarkerId == uid);

                reply = new MarkerInfoResponse { Id = uid.ToString() };
                reply.BuildContent(marker);

                if (GmcSettings.Get.CacheServices)
                    _memCache.Set(cacheKey, reply, TimeSpan.FromMinutes(10)); // cache data

                return reply;
            }
            catch (Exception ex)
            {
                return new MarkerInfoResponse
                {
                    OperationResult = "0",
                    ErrorMessage = string.Format("MapService says: Parsing error param: {0}", ex.Message)
                };
            }
        }
    }
}