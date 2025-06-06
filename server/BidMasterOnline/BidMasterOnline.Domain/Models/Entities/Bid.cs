namespace BidMasterOnline.Domain.Models.Entities
{
    public class Bid : EntityBase
    {
        public long AuctionId { get; set; }

        public long BidderId { get; set; }

        public decimal Amount { get; set; }

        public Auction? Auction { get; set; }

        public User? Bidder { get; set; }
    }
}
