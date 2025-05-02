using BidMasterOnline.Domain.Enums;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class ModeratorSummaryComplaintDTO
    {
        public long Id { get; set; }

        public long? ModeratorId { get; set; }

        public long AccusingUserId { get; set; }

        public required string Title { get; set; }

        public ComplaintType Type { get; set; }

        public ComplaintStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? ModeratorName { get; set; }
    }
}
