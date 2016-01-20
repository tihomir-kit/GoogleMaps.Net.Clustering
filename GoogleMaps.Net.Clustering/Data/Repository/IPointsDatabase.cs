using System.Collections.Generic;
using GoogleMaps.Net.Clustering.Data.Geometry;

namespace GoogleMaps.Net.Clustering.Contract
{
    public interface IPointsDatabase
    {
        IList<MapPoint> GetPoints();
    }
}
