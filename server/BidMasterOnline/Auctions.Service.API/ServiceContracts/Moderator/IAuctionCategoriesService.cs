using Auctions.Service.API.DTO.Moderator;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Moderator
{
    public interface IAuctionCategoriesService
    {
        Task<ServiceResult<PaginatedList<AuctionCategoryDTO>>> GetAuctionCategoriesAsync(SpecificationsDTO specifications);
        
        Task<ServiceResult> CreateAuctionCategoryAsync(UpsertAuctionCategoryDTO auctionCategoryDTO);

        Task<ServiceResult> UpdateAuctionCategoryAsync(long id, UpsertAuctionCategoryDTO auctionCategoryDTO);

        Task<ServiceResult> DeleteAuctionCategoryAsync(long id);
    }
}
