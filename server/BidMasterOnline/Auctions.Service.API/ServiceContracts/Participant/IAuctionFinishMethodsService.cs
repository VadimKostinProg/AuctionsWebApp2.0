using Auctions.Service.API.DTO.Participant;
using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.ServiceContracts.Participant
{
    public interface IAuctionFinishMethodsService
    {
        Task<ServiceResult<List<AuctionFinishMethodDTO>>> GetAuctionFinishMethodsAsync();
    }
}
