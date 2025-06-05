using BidMasterOnline.Domain.Enums;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class SummaryComplaintDTO
    {
        public long Id { get; set; }

        public long? ModeratorId { get; set; }

        public long AccusingUserId { get; set; }

        public required string Title { get; set; }

        public ComplaintStatus Status { get; set; }

        public DateTime SubmittedTime { get; set; }

        public required string AccusingUsername { get; set; }

        public string? ModeratorName { get; set; }
    }
}
