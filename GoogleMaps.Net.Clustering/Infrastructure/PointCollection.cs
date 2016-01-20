using System;
using System.Collections.Generic;
using GoogleMaps.Net.Clustering.Contract;
using System.Runtime.Caching;
using GoogleMaps.Net.Clustering.Data;
using GoogleMaps.Net.Clustering.Data.Geometry;
using GoogleMaps.Net.Clustering.Services;
using GoogleMaps.Net.Clustering.Utility;

namespace GoogleMaps.Net.Clustering.Infrastructure
{
    /// <summary>
    /// http://msdn.microsoft.com/library/system.runtime.caching.memorycache.aspx
    /// </summary>
    public class PointCollection : IPointCollection
    {
        protected readonly IMemCache _memCache;
        //public IMemCache MemCache => _memCache;

        protected readonly object _threadsafe = new object();

        public PointCollection()
        {
            _memCache = new MemCache();
        }

        public IList<MapPoint> Get(string pointType = null)
        {
            var key = GetPointKey(pointType);
            return _memCache.Get<IList<MapPoint>>(key);
        }

        /// <summary>
        /// Only add if not already added
        /// return whether it was added
        /// </summary>
        public bool Add(IList<MapPoint> points, TimeSpan timespan, string pointType = null)
        {
            var key = GetPointKey(pointType);
            return _memCache.Add(key, RandomizePointOrder(points), timespan);
        }

        public void Set(IList<MapPoint> points, TimeSpan timespan, string pointType = null)
        {
            var key = GetPointKey(pointType);
            _memCache.Set(key, RandomizePointOrder(points), timespan);
        }

        public object Remove(string pointType = null)
        {
            var key = GetPointKey(pointType);
            return _memCache.Remove(key);
        }

        public bool Exists(string pointType = null)
        {
            var key = GetPointKey(pointType);
            return _memCache.Exists(key);
        }

        /// <summary>
        /// Not really that important, could be ommited. Used only for ensuring visual 
        /// randomness of marker display when not all can be displayed on screen.
        ///
        /// Randomize order, when limit take is used for max marker display
        /// random locations are selected
        /// http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        /// </summary>
        /// <param name="points">Points to randomize.</param>
        /// <returns>Points in randomized order.</returns>
        private IList<MapPoint> RandomizePointOrder(IList<MapPoint> points)
        {
            lock (_threadsafe)
            {
                var rnd = new Random();
                for (var i = 0; i < points.Count; i++)
                {
                    MapPoint tmp = points[i];
                    int r = rnd.Next(points.Count);
                    points[i] = points[r];
                    points[r] = tmp;
                }

                return points;
            }
        }

        private static string GetPointKey(string pointType)
        {
            return String.Concat(CacheKeys.PointsDatabase, pointType);
        }
    }
}
