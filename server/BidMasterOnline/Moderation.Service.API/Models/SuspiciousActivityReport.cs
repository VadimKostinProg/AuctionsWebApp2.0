using MongoDB.Bson.Serialization.Attributes;

namespace Moderation.Service.API.Models
{
    [BsonIgnoreExtraElements]
    public class SuspiciousActivityReport
    {
        public required string ReportId { get; set; }
        public string Period { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<SuspiciousActivityReportAuctionAnalysis> AuctionAnalyses { get; set; } = new();
    }
}
