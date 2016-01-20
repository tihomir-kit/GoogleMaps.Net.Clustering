using System;
using GoogleMaps.Net.Clustering.Extensions;

namespace GoogleMaps.Net.Clustering.Data.Geometry
{
    [Serializable]
    public abstract class MapPointBase
    {
        private static int _counter;
        protected MapPointBase(){ Uid = ++_counter; Init();}
                     
        public virtual int Uid { get; private set; }
        public virtual object Data { get; set; } // Data container for anything
        
        public virtual double X { get { return Long; } set { Long = value; } }
        public virtual double Y { get { return Lat; } set { Lat = value; } }
        public virtual int Type { get { return MarkerType; } set { MarkerType = value; } }
        
        public virtual int Count { get; set; } // count
        public virtual int MarkerId { get; set; } // marker id           
        public virtual int MarkerType { get; set; } // marker type        
        public virtual string Name { get; set; } // custom

        private double _lat;
        public virtual double Lat
        {
            get { return _lat.Round(); }
            set { _lat = value; }
        }

        private double _long;
        public virtual double Long
        {
            get { return _long.Round(); }
            set { _long = value; }
        }

        void Init() { Count = 1; }

        public virtual double Distance(MapPointBase mapPoint)
        {
            return Distance(mapPoint.X, mapPoint.Y);
        }

        // Euclidean distance
        public virtual double Distance(double x, double y)
        {
            var dX = X - x;
            var dY = Y - y;
            var dist = (dX * dX) + (dY * dY);
            //dist = Math.Sqrt(dist);
            return dist;
        }

        public override bool Equals(object obj)
        {
            var o = obj as MapPointBase;
            return GetHashCode() == o?.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Uid.GetHashCode();
        }      
    }
}
