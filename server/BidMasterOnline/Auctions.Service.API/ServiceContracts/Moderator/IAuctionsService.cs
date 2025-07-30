using Auctions.Service.API.DTO.Moderator;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Moderator
{
    public interface IAuctionsService
    {
        Task<ServiceResult<PaginatedList<AuctionSummaryDTO>>> GetAuctionsListAsync(AuctionSpecificationsDTO specifications);
        Task<ServiceResult<PaginatedList<AuctionSummaryDTO>>> GetUserAuctionsAsync(long userId, PaginationRequestDTO pagination);
        Task<ServiceResult<AuctionDTO>> GetAuctionByIdAsync(long id);
        Task<ServiceResult> CancelAuctionAsync(CancelAuctionDTO requestDTO);
        Task<bool> CancelAllUserAuctionsAfterBlockingAsync(long userId);
        Task<ServiceResult> RecoverAuctionAsync(RecoverAuctionDTO requestDTO);
    }
}
