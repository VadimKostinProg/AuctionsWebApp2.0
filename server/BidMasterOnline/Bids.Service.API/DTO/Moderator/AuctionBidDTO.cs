using BidMasterOnline.Core.DTO;

namespace Bids.Service.API.DTO.Moderator
{
    public class AuctionBidDTO : BaseDTO
    {
        public long AuctionId { get; set; }

        public long BidderId { get; set; }

        public decimal Amount { get; set; }

        public string AuctionName { get; set; } = string.Empty;
    }
}
