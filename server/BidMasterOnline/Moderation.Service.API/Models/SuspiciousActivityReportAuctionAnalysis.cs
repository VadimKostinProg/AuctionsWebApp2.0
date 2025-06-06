namespace Moderation.Service.API.Models
{
    public class SuspiciousActivityReportAuctionAnalysis
    {
        public long AuctionId { get; set; }

        public string OverallAnalysisSummary { get; set; } = string.Empty;

        public List<ActivitySuspicion> Suspicions { get; set; } = new();
    }

    public class ActivitySuspicion
    {
        public string Type { get; set; } = string.Empty;

        public double ConfidenceScore { get; set; }

        public string Reasoning { get; set; } = string.Empty;

        public List<string> DetectedPatterns { get; set; } = new();

        public List<SuspiciousUser> InvolvedUsers { get; set; } = new();
    }

    public class SuspiciousUser
    {
        public long UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Reasoning { get; set; } = string.Empty;

        public List<long> RelatedBidIds { get; set; } = new();
    }
}
