using BidMasterOnline.Core.DTO;
using Moderation.Service.API.Enums;
using Moderation.Service.API.Models;

namespace Moderation.Service.API.ServiceContracts
{
    public interface ISuspiciousActivityReportsService
    {
        Task<ServiceResult<SuspiciousActivityReport>> GetSuspiciousActivityReportAsync(SuspiciousActivityReportPeriod period);
        Task<ServiceResult<SuspiciousActivityReportAuctionAnalysis>> GetAuctionAnalysisAsync(string analysisId);
    }
}
