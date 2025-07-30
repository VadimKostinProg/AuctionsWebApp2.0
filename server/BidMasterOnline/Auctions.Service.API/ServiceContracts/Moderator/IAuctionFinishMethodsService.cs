using Auctions.Service.API.DTO.Moderator;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Moderator
{
    public interface IAuctionFinishMethodsService
    {
        Task<ServiceResult<PaginatedList<AuctionFinishMethodDTO>>> GetAuctionFinishMethodsListAsync(SpecificationsDTO specifications);

        Task<ServiceResult> UpdateAuctionFinishMethodAsync(long id, UpdateAuctionFinishMethodDTO auctionFinishMethodDTO);
    }
}
