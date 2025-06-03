using BidMasterOnline.Core.DTO;

namespace Bids.Service.API.DTO.Participant
{
    public class AuctionBidDTO
    {
        public long AuctionId { get; set; }

        public decimal Amount { get; set; }

        public required string Time { get; set; }

        public required string BidderUsername { get; set; }

        public long BidderId { get; set; }
    }
}
