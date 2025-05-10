using Auctions.Service.API.DTO.Participant;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Participant
{
    public interface IParticipantAuctionCategoriesService
    {
        Task<ServiceResult<List<AuctionCategoryDTO>>> GetAuctionCategoriesAsync();
    }
}
