namespace BidMasterOnline.Domain.Models.Entities
{
    public class WatchList : EntityBase
    {
        public long UserId { get; set; }

        public long AuctionId { get; set; }

        public Auction? Auction { get; set; }
    }
}
