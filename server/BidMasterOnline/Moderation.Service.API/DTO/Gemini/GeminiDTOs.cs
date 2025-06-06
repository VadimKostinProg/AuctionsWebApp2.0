using System.Text.Json.Serialization;

namespace Moderation.Service.API.DTO.Gemini
{
    public class UserStats
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("userRegistrationDate")]
        public DateTime UserRegistrationDate { get; set; }

        [JsonPropertyName("totalWins")]
        public int TotalWins { get; set; }

        [JsonPropertyName("totalAuctionsOrganized")]
        public int TotalAuctionsOrganized { get; set; }

        [JsonPropertyName("totalAuctionsCompleted")]
        public int TotalAuctionsCompleted { get; set; }
    }

    public class Bid
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("bidderId")]
        public long BidderId { get; set; }

        [JsonPropertyName("bidder")]
        public UserStats Bidder { get; set; } = new();

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime PlacedAt { get; set; }
    }

    public class AuctionDetails
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("auctionistId")]
        public long AuctioneerId { get; set; }

        [JsonPropertyName("auctionist")]
        public UserStats Auctioneer { get; set; } = new();

        [JsonPropertyName("lotTitle")]
        public string LotTitle { get; set; } = string.Empty;

        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("finishTime")]
        public DateTime FinishTime { get; set; }

        [JsonPropertyName("bidAmountInterval")]
        public decimal BidAmountInterval { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("winnerId")]
        public long? WinnerId { get; set; }

        [JsonPropertyName("winner")]
        public UserStats? Winner { get; set; }

        [JsonPropertyName("finishPrice")]
        public decimal? FinishPrice { get; set; }
    }

    public class AuctionData
    {
        [JsonPropertyName("auctionDetails")]
        public AuctionDetails AuctionDetails { get; set; } = new();

        [JsonPropertyName("bidHistory")]
        public List<Bid> BidHistory { get; set; } = new();
    }

    public class GeminiInputPayload
    {
        [JsonPropertyName("auctionsToAnalyze")]
        public List<AuctionData> AuctionsToAnalyze { get; set; } = new();
    }

    // --- Output Data Classes ---

    public class InvolvedUser
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("reasoning")]
        public string Reasoning { get; set; } = string.Empty;

        [JsonPropertyName("relatedBidIds")]
        public List<long> RelatedBidIds { get; set; } = new();
    }

    public class Suspicion
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("confidenceScore")]
        public double ConfidenceScore { get; set; }

        [JsonPropertyName("reasoning")]
        public string Reasoning { get; set; } = string.Empty;

        [JsonPropertyName("detectedPatterns")]
        public List<string> DetectedPatterns { get; set; } = new();

        [JsonPropertyName("isPotentiallyProblematic")]
        public bool? IsPotentiallyProblematic { get; set; } // Only for Bid Sniping

        [JsonPropertyName("involvedUsers")]
        public List<InvolvedUser> InvolvedUsers { get; set; } = new();
    }

    public class AuctionAnalysisResult
    {
        [JsonPropertyName("auctionId")]
        public long AuctionId { get; set; }

        [JsonPropertyName("overallAnalysisSummary")]
        public string OverallAnalysisSummary { get; set; } = string.Empty;

        [JsonPropertyName("suspicions")]
        public List<Suspicion> Suspicions { get; set; } = new();
    }

    public class GeminiOutputPayload
    {
        [JsonPropertyName("auctionAnalyses")]
        public List<AuctionAnalysisResult> AuctionAnalyses { get; set; } = new();
    }

    // Gemini API specific classes (for wrapping input/output in the API call structure)
    public class GeminiContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiPart> Parts { get; set; } = new();
    }

    public class GeminiPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    public class GeminiRequest
    {
        [JsonPropertyName("contents")]
        public List<GeminiContent> Contents { get; set; } = new();
    }

    public class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent Content { get; set; } = new();
    }

    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate> Candidates { get; set; } = new();
    }
}
