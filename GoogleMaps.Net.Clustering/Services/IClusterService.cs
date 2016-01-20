using GoogleMaps.Net.Clustering.Data.Params;
using GoogleMaps.Net.Clustering.Data.Responses;

namespace GoogleMaps.Net.Clustering.Services
{
    public interface IClusterService
    {        
        ClusterMarkersResponse GetClusterMarkers(GetMarkersParams getParams);

        MarkerInfoResponse GetMarkerInfo(int uid, string pointType = null);
    }
}
