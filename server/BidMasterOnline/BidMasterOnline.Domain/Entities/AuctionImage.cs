namespace BidMasterOnline.Domain.Entities
{
    public class AuctionImage : EntityBase
    {
        public Guid AuctionId { get; set; }

        public required string Url { get; set; }

        public required string PublicId { get; set; }
    }
}
