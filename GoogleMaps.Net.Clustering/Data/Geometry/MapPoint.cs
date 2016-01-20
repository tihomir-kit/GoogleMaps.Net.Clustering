using GoogleMaps.Net.Clustering.Contract;
using GoogleMaps.Net.Clustering.Extensions;
using GoogleMaps.Net.Clustering.Utility;
using System;
using System.Runtime.Serialization;

namespace GoogleMaps.Net.Clustering.Data.Geometry
{
    /// <summary>
    /// Point class, overwrite it, modify it, extend it as you like
    /// </summary>    
    [Serializable]
    public class MapPoint : MapPointBase, ISerializable, IMapPoint
    {
        public MapPoint()
        {
        }

        public MapPoint(double x, double y) : this()
        {
            X = x;
            Y = y;
        }

        public virtual MapPoint Normalize()
        {
            Long = Long.NormalizeLongitude();
            Lat = Lat.NormalizeLatitude();
            return this;
        }

        // Dist between two points on Earth
        public new virtual double Distance(double x, double y)
        {
            return MathTool.Haversine(this.Y, this.X, y, x);
        }

        public override string ToString()
        {
            return String.Format("Uid: {0}, X:{1}, Y:{2}, MarkerType:{3}, MarkerId:{4}", Uid, X, Y, MarkerType, MarkerId);
        }

        // Used for e.g. serialization to file
        public MapPoint(SerializationInfo info, StreamingContext ctxt)
        {
            this.Count = 1;
            this.MarkerId = (int)info.GetValue("MarkerId", typeof(int));
            this.MarkerType = (int)info.GetValue("MarkerType", typeof(int));
            this.X = ((string)info.GetValue("X", typeof(string))).ToDouble();
            this.Y = ((string)info.GetValue("Y", typeof(string))).ToDouble();
        }

        // Data returned as Json
        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("MarkerId", this.MarkerId);
            info.AddValue("MarkerType", this.MarkerType);
            info.AddValue("X", this.X);
            info.AddValue("Y", this.Y);
            info.AddValue("Count", this.Count);
        }

        public int CompareTo(MapPoint other, int dimension)
        {
            switch (dimension)
            {
                case 0:
                    return this.X.CompareTo(other.X);
                case 1:
                    return this.Y.CompareTo(other.Y);
                default:
                    throw new ArgumentException("Invalid dimension.");
            }
        }
    }
}
