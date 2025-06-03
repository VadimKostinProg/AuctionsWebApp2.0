using Auctions.Service.API.DTO;
using Auctions.Service.API.DTO.Participant;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Participant
{
    public interface IAuctionsService
    {
        Task<ServiceResult<PaginatedList<AuctionSummaryDTO>>> GetAuctionsListAsync(AuctionSpecificationsDTO specifications);

        Task<ServiceResult<PaginatedList<AuctionSummaryDTO>>> GetUserAuctionsAsync(PaginationRequestDTO pagination);

        Task<ServiceResult<AuctionDTO>> GetAuctionByIdAsync(long id);

        Task<ServiceResult> CancelAuctionAsync(CancelAuctionDTO request);

        Task<bool> FinishAuctionAsync(long id);
    }
}
