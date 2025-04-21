using Auctions.Service.API.DTO;
using Auctions.Service.API.DTO.Moderator;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Moderator
{
    public interface IModeratorAuctionsService
    {
        Task<ServiceResult<PaginatedList<AuctionSummaryDTO>>> GetAuctionsListAsync(AuctionSpecificationsDTO specifications);
        Task<ServiceResult<AuctionDTO>> GetAuctionByIdAsync(long id);
        Task<bool> CancelAuctionAsync(long auctionId, string reason);
        Task<bool> RecoverAuctionAsync(long auctionId);
    }
}
