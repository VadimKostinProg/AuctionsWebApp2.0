using System.Net;
using System.Text.Json.Serialization;

namespace BidMasterOnline.Core.DTO
{
    public class ServiceResult
    {
        public string? Message { get; set; }

        public bool IsSuccessfull { get; set; } = true;

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public List<string> Errors { get; set; } = [];
    }

    public class ServiceResult<T>
    {
        public T? Data { get; set; }

        public bool IsSuccessfull { get; set; } = true;

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public List<string> Errors { get; set; } = [];
    }
}
