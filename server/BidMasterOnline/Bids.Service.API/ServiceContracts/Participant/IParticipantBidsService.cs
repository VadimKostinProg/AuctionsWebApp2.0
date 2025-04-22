using BidMasterOnline.Core.DTO;
using Bids.Service.API.DTO.Participant;

namespace Bids.Service.API.ServiceContracts.Participant
{
    public interface IParticipantBidsService
    {
        Task<ServiceResult> PostBidOnAuctionAsync(PostBidDTO bidDTO);
        Task<ServiceResult<PaginatedList<AuctionBidDTO>>> GetAuctionBidsAsync(long auctionId);
        Task<ServiceResult<PaginatedList<UserBidDTO>>> GetUserBidsAsync();
    }
}
