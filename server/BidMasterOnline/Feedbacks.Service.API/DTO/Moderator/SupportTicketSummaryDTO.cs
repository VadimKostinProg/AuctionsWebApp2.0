using BidMasterOnline.Domain.Enums;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class SupportTicketSummaryDTO
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long? ModeratorId { get; set; }

        public required string Title { get; set; }

        public SupportTicketStatus Status { get; set; }

        public DateTime SubmittedTime { get; set; }

        public string SubmittedUsername { get; set; } = string.Empty;

        public string? ModeratorName { get; set; }
    }
}
