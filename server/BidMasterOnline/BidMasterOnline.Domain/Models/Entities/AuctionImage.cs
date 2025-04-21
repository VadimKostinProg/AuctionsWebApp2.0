namespace BidMasterOnline.Domain.Models.Entities
{
    public class AuctionImage : EntityBase
    {
        public long? AuctionId { get; set; }

        public long? AuctionRequestId { get; set; }

        public required string Url { get; set; }

        public required string PublicId { get; set; }
    }
}
