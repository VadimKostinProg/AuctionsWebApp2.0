using Auctions.Service.API.DTO.Moderator;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Moderator
{
    public interface IModeratorAuctionTypesService
    {
        Task<ServiceResult<PaginatedList<ModeratorAuctionTypeDTO>>> GetAuctionTypesAsync(ModeratorSpecificationsDTO specifications);

        Task<ServiceResult> UpdateAuctionTypeAsync(long id, ModeratorUpdateAuctionTypeDTO auctionTypeDTO);
    }
}
