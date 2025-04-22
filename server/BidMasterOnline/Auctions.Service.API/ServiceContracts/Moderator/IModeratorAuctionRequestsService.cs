using Auctions.Service.API.DTO;
using Auctions.Service.API.DTO.Moderator;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Moderator
{
    public interface IModeratorAuctionRequestsService
    {
        Task<ServiceResult<PaginatedList<AuctionRequestSummaryDTO>>> GetAllAuctionRequestAsync(AuctionRequestSpecificationsDTO specifications); 
        Task<ServiceResult<AuctionRequestDTO>> GetAuctionRequestById(long id);
        Task<ServiceResult> ApproveAuctionRequestAsync(ApproveAuctionRequestDTO requestDTO);
        Task<ServiceResult> DeclineAuctionRequestAsync(DeclineAuctionRequestDTO requestDTO);
    }
}
