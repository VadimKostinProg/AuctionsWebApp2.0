using System.ComponentModel.DataAnnotations.Schema;

namespace BidMasterOnline.Domain.Entities
{
    public class Complaint : EntityBase
    {
        public Guid AccusingUserId { get; set; }

        public Guid AccusedUserId { get; set; }

        public Guid AuctionId { get; set; }

        public Guid? CommentId { get; set; }

        public Guid ComplaintTypeId { get; set; }

        public DateTime DateAndTime { get; set; }

        public required string ComplaintText { get; set; }

        public bool IsHandled { get; set; }

        [ForeignKey(nameof(AccusedUserId))]
        public User? AccusedUser { get; set; }

        [ForeignKey(nameof(AccusingUserId))]
        public User? AccusingUser { get; set; }

        [ForeignKey(nameof(AuctionId))]
        public Auction? Auction { get; set; }

        [ForeignKey(nameof(CommentId))]
        public AuctionComment? Comment { get; set; }

        [ForeignKey(nameof(ComplaintTypeId))]
        public ComplaintType? ComplaintType { get; set; }
    }
}
