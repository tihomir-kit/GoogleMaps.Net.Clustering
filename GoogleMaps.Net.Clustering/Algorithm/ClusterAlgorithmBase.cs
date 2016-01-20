using System;
using System.Collections.Generic;
using System.Linq;
using GoogleMaps.Net.Clustering.Data.Algo;
using GoogleMaps.Net.Clustering.Data.Configuration;
using GoogleMaps.Net.Clustering.Data.Geometry;
using GoogleMaps.Net.Clustering.Extensions;
using GoogleMaps.Net.Clustering.Utility;

namespace GoogleMaps.Net.Clustering.Algorithm
{
    /// <summary>
    /// /// Author: Kunuk Nykjaer
    /// </summary>
    internal abstract class ClusterAlgorithmBase
    {
        protected readonly IList<MapPoint> points;
        
        // id, bucket
        public readonly Dictionary<string, Bucket> BucketsLookup = new Dictionary<string, Bucket>();

        protected ClusterAlgorithmBase(IList<MapPoint> points)
        {
            this.points = points;
        }

        public List<MapPoint> GetClusterResult(Boundary grid)
        {
            // Collect used buckets and return the result
            var clusterPoints = new List<MapPoint>();

            // O(m*n)
            foreach (var item in BucketsLookup)
            {
                var bucket = item.Value;
                if (!bucket.IsUsed) continue;

                if (bucket.Points.Count < GmcSettings.Get.MinClusterSize)
                {
                    clusterPoints.AddRange(bucket.Points);
                }
                else
                {
                    bucket.Centroid.Count = bucket.Points.Count;
                    clusterPoints.Add(bucket.Centroid);
                }
            }

            // var filtered = FilterDataset(clusterPoints, grid); // post filter data for client viewport
            // return filtered; //not working properly when zoomed far out.
            return clusterPoints;  // return not post filtered
        }
        

        // Circular mean, very relevant for points around New Zealand, where lon -180 to 180 overlap
        // Adapted Centroid Calculation of N Points for Google Maps usage
        // If you are working with points in a specific country only such as India or USA then a simpler centroid calc can be used
        // http://en.wikipedia.org/wiki/Centroid
        public static MapPoint GetCentroidFromClusterLatLon(List<MapPoint> list) //O(n)
        {
            int count;
            if (list == null || (count = list.Count) == 0) return null;

            if (count == 1)
            {
                return list.First();
            }

            // http://en.wikipedia.org/wiki/Circular_mean
            // http://stackoverflow.com/questions/491738/how-do-you-calculate-the-average-of-a-set-of-angles
            /*
                                  1/N*  sum_i_from_1_to_N sin(a[i])
                a = atan2      ---------------------------
                                  1/N*  sum_i_from_1_to_N cos(a[i])
             */

            double longSin = 0;
            double longCos = 0;
            double latSin = 0;
            double latCos = 0;
            foreach (var p in list)
            {
                longSin += Math.Sin(p.X.LatLongToRadian());
                longCos += Math.Cos(p.X.LatLongToRadian());
                latSin += Math.Sin(p.Y.LatLongToRadian());
                latCos += Math.Cos(p.Y.LatLongToRadian());
            }

            longSin /= count;
            longCos /= count;

            double radx = 0;
            double rady = 0;

            // if both are zero-ish
            if (Math.Abs(longSin - 0) > Numbers.Epsilon && Math.Abs(longCos - 0) > Numbers.Epsilon)
            {
                radx = Math.Atan2(longSin, longCos);
                rady = Math.Atan2(latSin, latCos);
            }
            var x = radx.RadianToLatLong();
            var y = rady.RadianToLatLong();

            var centroid = new MapPoint { X = x, Y = y, Count = count };
            return centroid;
        }


        // O(k*n)
        public static void SetCentroidForAllBuckets(IEnumerable<Bucket> buckets)
        {
            foreach (var item in buckets)
            {
                item.Centroid = GetCentroidFromClusterLatLon(item.Points);
            }
        }

        public MapPoint GetClosestPoint(MapPoint from, List<MapPoint> list) // O(n)
        {
            var min = double.MaxValue;
            MapPoint closests = null;
            foreach (var p in list)
            {
                var dist = MathTool.Distance(from, p);
                if (dist >= min) continue;

                // update
                min = dist;
                closests = p;
            }
            return closests;
        }

        // Assign all points to nearest cluster
        public void UpdatePointsByCentroid() // O(n*k)
        {
            // Clear points in the buckets, they will be re-inserted
            foreach (var bucket in BucketsLookup.Values)
            {
                bucket.Points.Clear();
            }

            foreach (var p in this.points)
            {
                var minDist = Double.MaxValue;
                var index = string.Empty;
                foreach (var i in BucketsLookup.Keys)
                {
                    var bucket = BucketsLookup[i];
                    if (bucket.IsUsed == false)
                    {
                        continue;
                    }

                    var centroid = bucket.Centroid;
                    var dist = MathTool.Distance(p, centroid);
                    if (dist < minDist)
                    {
                        // update
                        minDist = dist;
                        index = i;
                    }
                }
                // re-insert
                var closestBucket = BucketsLookup[index];
                closestBucket.Points.Add(p);
            }
        }

        // Update centroid location to nearest point, 
        // e.g. if you want to show cluster point on a real existing point area
        // O(n)
        public void UpdateCentroidToNearestContainingPoint(Bucket bucket)
        {
            if (bucket == null || bucket.Centroid == null ||
                bucket.Points == null || bucket.Points.Count == 0)
            {
                return;
            }

            var closest = GetClosestPoint(bucket.Centroid, bucket.Points);
            bucket.Centroid.X = closest.X; // no normalize, points are already normalized by default
            bucket.Centroid.Y = closest.Y;
        }
        // O(k*n)
        public void UpdateAllCentroidsToNearestContainingPoint()
        {
            foreach (var bucket in BucketsLookup.Values)
            {
                UpdateCentroidToNearestContainingPoint(bucket);
            }
        }
    }
}
