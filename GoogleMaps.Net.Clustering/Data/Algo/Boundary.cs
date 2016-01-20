using System;
using GoogleMaps.Net.Clustering.Data.Geometry;
using GoogleMaps.Net.Clustering.Extensions;

namespace GoogleMaps.Net.Clustering.Data.Algo
{
    public class Boundary : Rectangle
    {        
        public Boundary() { }
        public Boundary(Rectangle rectangle)
        {            
            this.MinX = rectangle.MinX;
            this.MinY = rectangle.MinY;
            this.MaxX = rectangle.MaxX;
            this.MaxY = rectangle.MaxY;
        }


        /// <summary>
        /// Normalize lat and lon values to their boundary values
        /// O(1)
        /// </summary>
        public void Normalize()
        {
            MinX = MinX.NormalizeLongitude();
            MaxX = MaxX.NormalizeLongitude();
            MinY = MinY.NormalizeLatitude();
            MaxY = MaxY.NormalizeLatitude();
        }


        public void ValidateLatLon()
        {
            if ((MinX > LatLongInfo.MaxLonValue || MinX < LatLongInfo.MinLonValue)
                || (MaxX > LatLongInfo.MaxLonValue || MaxX < LatLongInfo.MinLonValue)
                || (MinY > LatLongInfo.MaxLatValue || MinY < LatLongInfo.MinLatValue)
                || (MaxY > LatLongInfo.MaxLatValue || MaxY < LatLongInfo.MinLatValue)
                )
                throw new Exception(string.Concat("input Boundary.ValidateLatLon() error ", this));
        }

        // Distance lon
        public double AbsX { get { return MinX.AbsLon(MaxX); } }

        // Distance lat
        public double AbsY { get { return MinY.AbsLat(MaxY); } }
    }
}
