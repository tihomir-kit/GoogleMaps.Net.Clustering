using System;
using GoogleMaps.Net.Clustering.Data.Algo;
using GoogleMaps.Net.Clustering.Data.Geometry;
using GoogleMaps.Net.Clustering.Extensions;

namespace GoogleMaps.Net.Clustering.Utility
{
    /// <summary>
    /// Author: Kunuk Nykjaer
    /// </summary>
    internal static class MathTool
    {
        const double Exp = 2; // 2=euclid, 1=manhatten
        
        // Minkowski dist
        // if lat lon precise dist is needed, use Haversine or similar formulas
        // this is approx calc for clustering, no precise dist is needed
        public static double Distance(MapPoint a, MapPoint b)
        {
            // lat lon wrap, values don't seem needed to be normalized to [0;1] for better distance calc
            var absx = LatLonDiff(a.X, b.X);
            var absy = LatLonDiff(a.Y, b.Y);

            return Math.Pow(Math.Pow(absx, Exp) +
                Math.Pow(Math.Abs(absy), Exp), 1.0 / Exp);
        }

        // O(1) while loop is maximum 2
        public static double LatLonDiff(double from, double to)
        {
            double difference = to - from;
            while (difference < -LatLongInfo.MaxLengthWrap) difference += LatLongInfo.MaxWorldLength;
            while (difference > LatLongInfo.MaxLengthWrap) difference -= LatLongInfo.MaxWorldLength;
            return Math.Abs(difference);

            //var differenceAngle = (to - from) % 180; //not working for -170 to 170
            //return Math.Abs(differenceAngle);
        }

        public static double Haversine(MapPoint p1, MapPoint p2)
        {
            return Haversine(p1.Y, p1.X, p2.Y, p2.X);
        }

        // http://en.wikipedia.org/wiki/Haversine_formula
        // Approx dist between two points on earth
        //public static double Haversine(double lat1, double lon1, double lat2, double lon2)
        //{
        //    const int r = 6371; // km
        //    var dLat = ToRadians(lat2 - lat1);
        //    var dLon = ToRadians(lon2 - lon1);
        //    lat1 = ToRadians(lat1);
        //    lat2 = ToRadians(lat2);

        //    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
        //            Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        //    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        //    var d = r * c;
        //    return d;
        //}
        // http://en.wikipedia.org/wiki/Haversine_formula
        // Approx dist between two points on earth
        // http://rosettacode.org/wiki/Haversine_formula
        public static double Haversine(double lat1, double long1, double lat2, double long2)
        {
            const double r = 6372.8; // In kilometers
            var dLat = ToRadians(lat2 - lat1);
            var dLong = ToRadians(long2 - long1);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + 
                Math.Sin(dLong / 2) * Math.Sin(dLong / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            var d = r * c;
            return d;
        }

        public static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static bool IsLowerThanLatMin(double d)
        {
            return d < LatLongInfo.MinLatValue;
        }
        public static bool IsGreaterThanLatMax(double d)
        {
            return d > LatLongInfo.MaxLatValue;
        }

        /// <summary>
        /// Lat Lon specific rect boundary check, is x,y inside boundary?
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// /// <param name="isInsideDetectedX"></param>
        /// /// <param name="isInsideDetectedY"></param>
        /// <returns></returns>
        public static bool IsInside(double minX, double minY, double maxX, double maxY, double x, double y, bool isInsideDetectedX, bool isInsideDetectedY)
        {
            // Normalize because of widen function, world wrapping might have occured
            // calc in positive value range only, nb. lon -170 = 10, lat -80 = 10
            var nMinX = minX.NormalizeLongitude();
            var nMaxX = maxX.NormalizeLongitude();

            var nMinY = minY.NormalizeLatitude();
            var nMaxY = maxY.NormalizeLatitude();

            var nX = x.NormalizeLongitude();
            var nY = y.NormalizeLatitude();

            bool isX = isInsideDetectedX; // skip checking?
            bool isY = isInsideDetectedY;

            if (!isInsideDetectedY)
            {
                // world wrap y
                if (nMinY > nMaxY)
                {
                    //sign depended check, todo merge equal lines
                    // - -
                    if (nMaxY <= 0 && nMinY <= 0)
                    {
                        isY = nMinY <= nY && nY <= LatLongInfo.MaxLatValue || LatLongInfo.MinLatValue <= nY && nY <= nMaxY;
                    }
                    // + +
                    else if (nMaxY >= 0 && nMinY >= 0)
                    {
                        isY = nMinY <= nY && nY <= LatLongInfo.MaxLatValue || LatLongInfo.MinLatValue <= nY && nY <= nMaxY;
                    }
                    // + -
                    else
                    {
                        isY = nMinY <= nY && nY <= LatLongInfo.MaxLatValue || LatLongInfo.MinLatValue <= nY && nY <= nMaxY;
                    }
                }

                else
                {
                    // normal, no world wrap 
                    isY = nMinY <= nY && nY <= nMaxY;
                }
            }

            if (!isInsideDetectedX)
            {
                // world wrap x
                if (nMinX > nMaxX)
                {
                    //sign depended check, todo merge equal lines
                    // - -
                    if (nMaxX <= 0 && nMinX <= 0)
                    {
                        isX = nMinX <= nX && nX <= LatLongInfo.MaxLonValue || LatLongInfo.MinLonValue <= nX && nX <= nMaxX;
                    }                        
                    // + +
                    else if (nMaxX >= 0 && nMinX >= 0)
                    {
                        isX = nMinX <= nX && nX <= LatLongInfo.MaxLonValue || LatLongInfo.MinLonValue <= nX && nX <= nMaxX;
                    }
                    // + -
                    else
                    {
                        isX = nMinX <= nX && nX <= LatLongInfo.MaxLonValue || LatLongInfo.MinLonValue <= nX && nX <= nMaxX;
                    }
                }
                else
                {
                    // normal, no world wrap
                    isX = nMinX <= nX && nX <= nMaxX;
                }
            }

            return isX && isY;
        }

        public static bool IsInside(Boundary b, MapPoint p)
        {
            return IsInside(b.MinX, b.MinY, b.MaxX, b.MaxY, p.X, p.Y, false, false);
        }
        
        // used by zoom level and deciding the grid size, O(halfSteps)
        // O(halfSteps) ~  O(maxzoom) ~  O(k) ~  O(1)
        // Google Maps doubles or halves the view for 1 step zoom level change
        public static double Half(double d, int halfSteps)
        {
            // http://en.wikipedia.org/wiki/Decimal_degrees
            const double meter11 = 0.0001; //decimal degrees

            double half = d;
            for (int i = 0; i < halfSteps; i++)
            {
                half /= 2;
            }
                
            var halfRounded = Math.Round(half, 4);
            // avoid grid span less than this level
            return halfRounded < meter11 ? meter11 : halfRounded;
        }

        // Value x which is in range [a,b] is mapped to a new value in range [c;d]
        public static double Map(double x, double a, double b, double c, double d)
        {
            var r = (x - a) / (b - a) * (d - c) + c;
            return r;
        }

        // Grid location are stationary, this gives first left or lower grid line from current latOrLon
        public static double FloorLatLon(double latOrlon, double delta)
        {
            var floor = ((int)(latOrlon / delta)) * delta;
            if (latOrlon < 0) floor -= delta;

            return floor;
        }

        // 
        public static bool IsValidLat(double latitude)
        {
            return LatLongInfo.MinLatValue <= latitude && latitude <= LatLongInfo.MaxLatValue;
        }
        public static bool IsValidLong(double longitude)
        {
            return LatLongInfo.MinLonValue <= longitude && longitude <= LatLongInfo.MaxLonValue;
        }

        // Value must be within a and b
        public static double Constrain(double x, double a, double b)
        {
            var r = Math.Max(a, Math.Min(x, b));
            return r;
        }

        // Value must be within latitude boundary        
        public static double ConstrainLatitude(double x, double offset = 0)
        {            
            var r = Math.Max(LatLongInfo.MinLatValue + offset, Math.Min(x, LatLongInfo.MaxLatValue - offset));
            return r;
        }
    }
}
