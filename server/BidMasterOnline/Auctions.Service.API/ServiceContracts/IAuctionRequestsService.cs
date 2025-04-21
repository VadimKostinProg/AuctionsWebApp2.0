using Auctions.Service.API.DTO;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts
{
    public interface IAuctionRequestsService
    {
        Task<Result<List<AuctionRequestSummaryDTO>>> GetUserAuctionRequests();
        Task<Result<AuctionRequestDTO>> GetUserAuctionRequestById(long id);
        Task<Result<string>> PostAuctionRequest(PostAuctionRequestDTO requestDTO);
        Task<Result<string>> CancelAuctionRequestById(long id);
    }
}
