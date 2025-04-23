using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Models.Entities;

namespace Bids.Service.API.ServiceContracts.Participant
{
    public interface IBidsPlacingStrategy
    {
        ServiceResult PlaceNewBid(Bid newBid, Auction auction);
    }
}
