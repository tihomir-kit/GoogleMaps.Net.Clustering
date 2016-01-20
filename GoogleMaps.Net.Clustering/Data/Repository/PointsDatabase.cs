using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GoogleMaps.Net.Clustering.Contract;
using GoogleMaps.Net.Clustering.Data.Configuration;
using GoogleMaps.Net.Clustering.Data.Geometry;
using GoogleMaps.Net.Clustering.Extensions;
using GoogleMaps.Net.Clustering.Infrastructure;
using GoogleMaps.Net.Clustering.Services;

namespace GoogleMaps.Net.Clustering.Data.Repository
{
    /// <summary>
    /// The database for all the existing points
    /// </summary>
    public class PointsDatabase : IPointsDatabase
    {
        public readonly string FilePath;

        protected readonly IMemCache _memCache;

        protected IList<MapPoint> Points { get; set; }

        protected readonly object _threadsafe = new object();

        public IList<MapPoint> GetPoints()
        {
            return Points;
        }

        public PointsDatabase(IMemCache memCache, IList<MapPoint> points)
        {
            _memCache = memCache;

            Points = _memCache.Get<IList<MapPoint>>(CacheKeys.PointsDatabase);
            if (Points != null) return; // cache hit

            lock (_threadsafe)
            {
                //Points = _memCache.Get<IList<MapPoint>>(CacheKeys.PointsDatabase);
                //if (Points != null) return;

                Points = new List<MapPoint>();

                // Not important, can be deleted, only for ensuring visual randomness of marker display 
                // when not all can be displayed on screen
                //
                // Randomize order, when limit take is used for max marker display
                // random locations are selected
                // http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
                var rnd = new Random();
                var count = points.Count;
                for (var i = 0; i < count; i++)
                {
                    MapPoint tmp = points[i];
                    int r = rnd.Next(count);
                    points[i] = points[r];
                    points[r] = tmp;
                }

                Points = points;

                _memCache.Set<IList<MapPoint>>(CacheKeys.PointsDatabase, Points, TimeSpan.FromHours(24));

                var data = _memCache.Get<IList<MapPoint>>(CacheKeys.PointsDatabase);
                if (data == null)
                {
                    throw new Exception("cache not working");
                }
            }
        }
    }
}
