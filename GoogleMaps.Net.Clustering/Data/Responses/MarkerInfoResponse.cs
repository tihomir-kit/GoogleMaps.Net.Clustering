using System;
using System.Text;
using GoogleMaps.Net.Clustering.Data.Geometry;

namespace GoogleMaps.Net.Clustering.Data.Responses
{
    public class MarkerInfoResponse : ResponseBase
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public int Type  { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }

        public void BuildContent(MapPoint point)
        {
            if (point == null)
            {
                Content = "Marker could not be found";
                return;
            }

            Id = point.MarkerId.ToString();
            Type = point.MarkerType;
            Lat = point.Lat;
            Long = point.Long;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<div class='gmcKN-marker-info'>");
            stringBuilder.AppendFormat("Time: {0}<br/>",DateTime.Now.ToString("HH:mm:ss"));
            stringBuilder.AppendFormat("Id: {0}<br /> Type: {1}<br />", Id, Type);
            stringBuilder.AppendFormat("Lat: {0} Lon: {1}", point.Lat, point.Long);
            stringBuilder.AppendLine("</div>");

            Content = stringBuilder.ToString();
        }        
    }
}
