using System.Collections.Generic;
using GoogleMaps.Net.Clustering.Data.Geometry;

namespace GoogleMaps.Net.Clustering.Data.Algo
{
    internal class Bucket
    {
        public string Id { get; private set; }

        public List<MapPoint> Points { get; private set; }

        public MapPoint Centroid { get; set; }

        public int Idx { get; private set; }

        public int Idy { get; private set; }

        public double ErrorLevel { get; set; } // clusterpoint and points avg dist

        private bool _isUsed;

        public bool IsUsed
        {
            get { return _isUsed && Centroid != null; }
            set { _isUsed = value; }
        }

        public Bucket(string id)
        {
            IsUsed = true;
            Centroid = null;
            Points = new List<MapPoint>();
            Id = id;
        }

        public Bucket(int idx, int idy, string id)
        {
            IsUsed = true;
            Centroid = null;
            Points = new List<MapPoint>();
            Idx = idx;
            Idy = idy;
            Id = id;
        }
    }
}
