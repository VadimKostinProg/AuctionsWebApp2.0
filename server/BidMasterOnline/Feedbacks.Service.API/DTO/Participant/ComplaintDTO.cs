namespace Feedbacks.Service.API.DTO.Participant
{
    public class ComplaintDTO : SummaryComplaintDTO
    {
        public long AccusedUserId { get; set; }

        public long? AccusedAuctionId { get; set; }

        public long? AccusedCommentId { get; set; }

        public long? AccusedUserFeedbackId { get; set; }

        public required string ComplaintText { get; set; }

        public string? ModeratorConclusion { get; set; }
    }
}
