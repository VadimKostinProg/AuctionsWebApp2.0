using Auctions.Service.API.DTO;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Models;

namespace Auctions.Service.API.ServiceContracts.Participant
{
    public interface IAuctionsService
    {
        Task<ServiceResult<ListModel<AuctionSummaryDTO>>> GetAuctionsListAsync(AuctionSpecificationsDTO specifications);

        Task<ServiceResult<AuctionDTO>> GetAuctionByIdAsync(long id);

        Task<ServiceResult> CancelAuctionAsync(long id);
    }
}
