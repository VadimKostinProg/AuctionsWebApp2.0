using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Models.Entities
{
    public class AuctionComment : EntityBase
    {
        public long UserId { get; set; }

        public long AuctionId { get; set; }

        [Range(1, 10)]
        public int Score { get; set; }

        [MaxLength(5000)]
        public required string CommentText { get; set; }

        public User? User { get; set; }

        public Auction? Auction { get; set; }
    }
}
