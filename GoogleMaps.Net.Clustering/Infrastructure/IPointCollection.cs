using System;
using System.Collections.Generic;
using GoogleMaps.Net.Clustering.Data.Geometry;

namespace GoogleMaps.Net.Clustering.Infrastructure
{
    public interface IPointCollection
    {
        IList<MapPoint> Get(string pointType = null);

        bool Add(IList<MapPoint> points, TimeSpan timespan, string pointType = null);

        void Set(IList<MapPoint> points, TimeSpan timespan, string pointType = null);

        object Remove(string pointType = null);

        bool Exists(string pointType = null);
    }
}
