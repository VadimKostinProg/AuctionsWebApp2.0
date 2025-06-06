using Moderation.Service.API.DTO.Gemini;
using Moderation.Service.API.Models;

namespace Moderation.Service.API.Extensions
{
    public static class ModelMappingExtensions
    {
        public static SuspiciousActivityReport ToModel(this GeminiOutputPayload payload)
            => new SuspiciousActivityReport()
            {
                ReportId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                AuctionAnalyses = payload.AuctionAnalyses.Where(a => a.Suspicions.Any())
                .Select(a => new SuspiciousActivityReportAuctionAnalysis
                {
                    AuctionId = a.AuctionId,
                    OverallAnalysisSummary = a.OverallAnalysisSummary,
                    Suspicions = a.Suspicions.Select(s => new ActivitySuspicion
                    {
                        Type = s.Type,
                        ConfidenceScore = s.ConfidenceScore,
                        Reasoning = s.Reasoning,
                        DetectedPatterns = s.DetectedPatterns,
                        InvolvedUsers = s.InvolvedUsers.Select(u => new SuspiciousUser
                        {
                            UserId = u.Id,
                            Username = u.Username,
                            Reasoning = u.Reasoning,
                            Role = u.Role,
                            RelatedBidIds = u.RelatedBidIds,
                        })
                        .ToList()
                    })
                    .ToList()
                })
                .ToList(),
            };
    }
}
