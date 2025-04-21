using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Moderation.Service.API.ServiceContracts
{
    public interface IModerationLogger
    {
        Task<ServiceResult> LogAction(ModerationAction action,
            long? userId = null,
            long? auctionId = null,
            long? auctionRequestId = null,
            long? commentId = null,
            long? complaintId = null,
            long? technicalRequestId = null);
    }
}
