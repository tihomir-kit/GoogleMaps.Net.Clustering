using Newtonsoft.Json;

namespace GoogleMaps.Net.Clustering.Data.Responses
{
    public abstract class ResponseBase
    {
        /// <summary>
        /// Server-side calculation elapsed time.
        /// </summary>
        public string Elapsed { get; set; }

        /// <summary>
        /// Operation result.
        /// </summary>
        public string OperationResult { get; set; }

        /// <summary>
        /// Error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Dev info.
        /// </summary>
        public string Debug { get; set; }
        
        /// <summary>
        /// Is data cached?
        /// </summary>
        [JsonProperty(Order = -2)] // makes it appear first in json response
        public bool? IsCached { get; set; }

        protected ResponseBase()
        {            
            OperationResult = "1";
        }
    }
}
