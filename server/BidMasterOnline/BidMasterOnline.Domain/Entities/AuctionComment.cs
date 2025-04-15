using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BidMasterOnline.Domain.Entities
{
    public class AuctionComment : EntityBase 
    {
        public Guid UserId { get; set; }

        public Guid AuctionId { get; set; }

        public DateTime DateAndTime { get; set; }

        [MaxLength(300)]
        public required string CommentText { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(AuctionId))]
        public Auction? Auction { get; set; }
    }
}
