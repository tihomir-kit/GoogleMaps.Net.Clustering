using System;
using GoogleMaps.Net.Clustering.Contract;
using System.Runtime.Caching;
using GoogleMaps.Net.Clustering.Services;

namespace GoogleMaps.Net.Clustering.Infrastructure
{
    /// <summary>
    /// http://msdn.microsoft.com/library/system.runtime.caching.memorycache.aspx
    /// </summary>
    public class MemCache : IMemCache // TODO: make internal?
    {
        private readonly ObjectCache _cache = MemoryCache.Default;

        public  T Get<T>(string key) where T : class
        {
            var cachedObj = _cache[key];
            if (cachedObj == null)
                return default(T);

            return cachedObj as T;
        }

        /// <summary>
        /// Only add if not already added
        /// return whether it was added
        /// </summary>
        public  bool Add<T>(string key, T objectToCache, TimeSpan timespan)
        {
            return _cache.Add(key, objectToCache, DateTime.Now.Add(timespan));
        }

        public void Set<T>(string key, T objectToCache, TimeSpan timespan)
        {
            _cache.Set(key, objectToCache, DateTime.Now.Add(timespan));
        }

        public  object Remove(string key)
        {
            return _cache.Remove(key);
        }
        public  bool Exists(string key)
        {               
            return _cache.Get(key) != null;
        }	
    }
}
