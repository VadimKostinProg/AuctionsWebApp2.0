using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class AuctionComment : EntityBase 
    {
        public long UserId { get; set; }

        public long AuctionId { get; set; }

        [MaxLength(5000)]
        public required string CommentText { get; set; }

        public User? User { get; set; }

        public Auction? Auction { get; set; }
    }
}
