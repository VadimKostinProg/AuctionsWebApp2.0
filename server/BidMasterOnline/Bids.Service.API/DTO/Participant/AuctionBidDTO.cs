using BidMasterOnline.Core.DTO;

namespace Bids.Service.API.DTO.Participant
{
    public class AuctionBidDTO
    {
        public long AuctionId { get; set; }

        public decimal Amount { get; set; }

        public DateTime Time { get; set; }

        public required UserSummaryDTO Bidder { get; set; }
    }
}
