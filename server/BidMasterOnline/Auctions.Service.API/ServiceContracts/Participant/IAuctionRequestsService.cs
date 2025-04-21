using Auctions.Service.API.DTO.Participant;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Participant
{
    public interface IAuctionRequestsService
    {
        Task<ServiceResult<PaginatedList<AuctionRequestSummaryDTO>>> GetUserAuctionRequestsAsync(PaginationRequestDTO pagination);
        Task<ServiceResult<AuctionRequestDTO>> GetAuctionRequestByIdAsync(long id);
        Task<ServiceResult> PostAuctionRequestAsync(PostAuctionRequestDTO requestDTO);
        Task<ServiceResult> CancelAuctionRequestByIdAsync(long id);
    }
}
