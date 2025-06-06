using BidMasterOnline.Core.DTO;
using Moderation.Service.API.Enums;

namespace Moderation.Service.API.ServiceContracts
{
    public interface ISuspiciousActivityCheckService
    {
        Task<ServiceResult<object>> GenerateSuspiciousActivityReportAsync(SuspiciousActivityReportPeriod period);
    }
}
