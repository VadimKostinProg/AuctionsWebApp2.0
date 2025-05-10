using Auctions.Service.API.DTO.Participant;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Participant
{
    public interface IParticipantAuctionTypesService
    {
        Task<ServiceResult<List<AuctionTypeDTO>>> GetAuctionTypesAsync();
    }
}
