using Auctions.Service.API.DTO.Moderator;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Moderator
{
    public interface IModeratorAuctionFinishMethodsService
    {
        Task<ServiceResult<PaginatedList<ModeratorAuctionFinishMethodDTO>>> GetAuctionFinishMethodsAsync(ModeratorSpecificationsDTO specifications);

        Task<ServiceResult> UpdateAuctionFinishMethodAsync(long id, ModeratorUpdateAuctionFinishMethodDTO auctionFinishMethodDTO);
    }
}
