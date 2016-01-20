using System;

namespace GoogleMaps.Net.Clustering.Data.Geometry
{
    [Serializable]
    public class Rectangle
    {
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }

        public override string ToString()
        {
            return String.Format("minx: {0} miny: {1} maxx: {2} maxy: {3}", MinX, MinY, MaxX, MaxY);
        }

        public override int GetHashCode()
        {
            return String.Format("{0}_{1}_{2}_{3}", MinX, MaxX, MinY, MaxY).GetHashCode();
        }
    }
}
