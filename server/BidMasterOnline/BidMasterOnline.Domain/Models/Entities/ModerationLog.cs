using BidMasterOnline.Domain.Enums;

namespace BidMasterOnline.Domain.Models.Entities
{
    public class ModerationLog : EntityBase
    {
        public long? UserId { get; set; }

        public long? AuctionId { get; set; }

        public long? AuctionRequestId { get; set; }

        public long? AuctionCommentId { get; set; }

        public long? ComplaintId { get; set; }

        public long? TechnicalSupportRequestId { get; set; }

        public ModerationAction Action { get; set; }
    }
}
