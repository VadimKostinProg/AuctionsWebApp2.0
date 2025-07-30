using BidMasterOnline.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Models.Entities
{
    public class Complaint : EntityBase
    {
        public long AccusingUserId { get; set; }

        public long AccusedUserId { get; set; }

        public long? AccusedAuctionId { get; set; }

        public long? AccusedCommentId { get; set; }

        public long? AccusedUserFeedbackId { get; set; }

        public long? ModeratorId { get; set; }

        [MaxLength(10000)]
        public required string ComplaintText { get; set; }

        public ComplaintType Type { get; set; }

        public ComplaintStatus Status { get; set; }

        [MaxLength(10000)]
        public string? ModeratorConclusion { get; set; }

        public User? Moderator { get; set; }

        public User? AccusingUser { get; set; }

        public User? AccusedUser { get; set; }

        public Auction? AccusedAuction { get; set; }

        public AuctionComment? AccusedComment { get; set; }

        public UserFeedback? AccusedUserFeedback { get; set; }
    }
}
