using BidMasterOnline.Domain.Enums;

namespace Feedbacks.Service.API.DTO.Participant
{
    public class ParticipantComplaintDTO : ParticipantSummaryComplaintDTO
    {
        public long AccusedUserId { get; set; }

        public long? AccusedAuctionId { get; set; }

        public long? AccusedCommentId { get; set; }

        public long? AccusedUserFeedbackId { get; set; }

        public required string ComplaintText { get; set; }

        public ComplaintStatus Status { get; set; }

        public string ModeratorConclusion { get; set; } = string.Empty;
    }
}
