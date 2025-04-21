using Auctions.Service.API.DTO;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Participant
{
    public interface IAuctionRequestsService
    {
        Task<ServiceResult<List<AuctionRequestSummaryDTO>>> GetUserAuctionRequests();
        Task<ServiceResult<AuctionRequestDTO>> GetUserAuctionRequestById(long id);
        Task<ServiceResult> PostAuctionRequest(PostAuctionRequestDTO requestDTO);
        Task<ServiceResult> CancelAuctionRequestById(long id);
    }
}
