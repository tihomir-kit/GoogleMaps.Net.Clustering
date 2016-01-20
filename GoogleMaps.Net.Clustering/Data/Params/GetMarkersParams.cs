namespace GoogleMaps.Net.Clustering.Data.Params
{
    public class GetMarkersParams
    {
        public double NorthEastLatitude { get; set; }

        public double NorthEastLongitude { get; set; }

        public double SouthWestLatitude { get; set; }

        public double SouthWestLongitude { get; set; }

        public int ZoomLevel { get; set; }

        public string Filter { get; set; }

        public string ClientWidth { get; set; }

        public string ClientHeigth { get; set; }

        /// <summary>
        /// Point type (optional). Used if you have different types of points on the map.
        /// For example - "Library", "Shop", "Hospital"..
        /// </summary>
        public string PointType { get; set; }
    }
}
