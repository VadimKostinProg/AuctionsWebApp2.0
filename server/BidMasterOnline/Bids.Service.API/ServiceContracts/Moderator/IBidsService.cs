using BidMasterOnline.Core.DTO;
using Bids.Service.API.DTO.Moderator;

namespace Bids.Service.API.ServiceContracts.Moderator
{
    public interface IBidsService
    {
        Task<bool> ClearAllBidsForAuctionAsync(long auctionId);
        Task<ServiceResult<PaginatedList<AuctionBidDTO>>> GetAuctionBidsAsync(long auctionId);
        Task<ServiceResult<PaginatedList<UserBidDTO>>> GetUserBidsAsync(long userId);
    }
}
