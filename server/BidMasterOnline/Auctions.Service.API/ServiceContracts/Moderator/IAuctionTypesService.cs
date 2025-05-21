using Auctions.Service.API.DTO.Moderator;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Moderator
{
    public interface IAuctionTypesService
    {
        Task<ServiceResult<PaginatedList<AuctionTypeDTO>>> GetAuctionTypesAsync(SpecificationsDTO specifications);

        Task<ServiceResult> UpdateAuctionTypeAsync(long id, UpdateAuctionTypeDTO auctionTypeDTO);
    }
}
