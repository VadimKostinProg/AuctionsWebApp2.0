using BidMasterOnline.Domain.Models.Entities;

namespace Bids.Service.API.ServiceContracts.Participant
{
    public interface IBidsPlacingStrategyFactory
    {
        IBidsPlacingStrategy GetStategyByAuctionType(AuctionType auctionType);
    }
}
