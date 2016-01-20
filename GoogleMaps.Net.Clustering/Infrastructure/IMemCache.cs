using System;

namespace GoogleMaps.Net.Clustering.Infrastructure
{
    public interface IMemCache // TODO: make internal?
    {
        T Get<T>(string key) where T : class;

        bool Add<T>(string key, T objectToCache, TimeSpan timespan);

        void Set<T>(string key, T objectToCache, TimeSpan timespan);

        object Remove(string key);

        bool Exists(string key);
    }
}
