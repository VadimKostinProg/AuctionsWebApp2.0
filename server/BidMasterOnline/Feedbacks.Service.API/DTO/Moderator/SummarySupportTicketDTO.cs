using BidMasterOnline.Domain.Enums;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class SummarySupportTicketDTO
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long? ModeratorId { get; set; }

        public required string Title { get; set; }

        public SupportTicketStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public string SubmitUsername { get; set; } = string.Empty;

        public string? ModeratorName { get; set; }
    }
}
