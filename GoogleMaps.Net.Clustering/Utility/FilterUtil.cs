using System.Collections.Generic;
using System.Linq;
using GoogleMaps.Net.Clustering.Data;
using GoogleMaps.Net.Clustering.Data.Algo;
using GoogleMaps.Net.Clustering.Data.Configuration;
using GoogleMaps.Net.Clustering.Data.Geometry;
using GoogleMaps.Net.Clustering.Extensions;

namespace GoogleMaps.Net.Clustering.Utility
{
    internal static class FilterUtil
    {
        /// <summary>
        /// Supports threads
        /// </summary>
        /// <param name="points"></param>
        /// <param name="filterData"></param>
        /// <returns></returns>
        public static IList<MapPoint> FilterByType(IList<MapPoint> points, FilterData filterData)
        {
            if (filterData.TypeFilterExclude.Count == GmcSettings.Get.MarkerTypes.Count)
            {
                // Filter all 
                return new List<MapPoint>(); // empty
            }
            if (filterData.TypeFilterExclude.None())
            {
                // Filter none
                return points;
            }

            // Filter data by typeFilter value
            return FilterByTypeHelper(points, filterData);
        }

        // O(n)
        public static IList<MapPoint> FilterDataByViewport(IList<MapPoint> points, Boundary viewport)
        {
            return points
                 .Where(i => MathTool.IsInside(viewport, i))
                 .ToList();
        }

        // O(n)
        private static IList<MapPoint> FilterByTypeHelper(IList<MapPoint> points, FilterData filterData)
        {
            return points
                .Where(p => filterData.TypeFilterExclude.Contains(p.MarkerType) == false)
                .ToList();
        }
    }
}
