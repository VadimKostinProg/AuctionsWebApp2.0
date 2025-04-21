using BidMasterOnline.Core.DTO;
using Moderation.Service.API.DTO;

namespace Moderation.Service.API.ServiceContracts
{
    public interface IAuctionsService
    {
        Task<ServiceResult> ApproveAuctionRequestAsync(ApproveAuctionRequestDTO requestDTO);
        Task<ServiceResult> DeclineAuctionRequestAsync(DeclineAuctionRequestDTO requestDTO);
        Task<ServiceResult> CancelAuctionAsync(CancelAuctionDTO requestDTO);
        Task<ServiceResult> RecoverAuctionAsync(RecoverAuctionDTO requestDTO);
    }
}
