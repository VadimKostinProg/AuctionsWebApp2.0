using BidMasterOnline.Core.Constants;
using BidMasterOnline.Domain.Models.Entities;
using Bids.Service.API.ServiceContracts.Participant;

namespace Bids.Service.API.Services.Participant
{
    public class BidsPlacingStrategyFactory : IBidsPlacingStrategyFactory
    {
        private readonly Dictionary<string, IBidsPlacingStrategy> _strategies = new()
        {
            { AuctionTypes.EnglishAuction, new EnglishBidsPlacingStrategy() },
            { AuctionTypes.DutchAuction, new DutchBidsPlacingStrategy() },
        };

        public IBidsPlacingStrategy GetStategyByAuctionType(AuctionType auctionType)
            => _strategies[auctionType.Name];
    }
}
