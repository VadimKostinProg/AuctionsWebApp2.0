using System.Net;
using System.Text.Json.Serialization;

namespace BidMasterOnline.Core.DTO
{
    public class Result<T>
    {
        public T? Data { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public List<string> Errors { get; set; } = [];
    }
}
