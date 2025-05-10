using Auctions.Service.API.DTO.Moderator;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Moderator
{
    public interface IModeratorAuctionCategoriesService
    {
        Task<ServiceResult<PaginatedList<ModeratorAuctionCategoryDTO>>> GetAuctionCategoriesAsync(ModeratorSpecificationsDTO specifications);
        
        Task<ServiceResult> CreateAuctionCategoryAsync(ModeratorUpsertAuctionCategoryDTO auctionCategoryDTO);

        Task<ServiceResult> UpdateAuctionCategoryAsync(long id, ModeratorUpsertAuctionCategoryDTO auctionCategoryDTO);

        Task<ServiceResult> DeleteAuctionCategoryAsync(long id);
    }
}
