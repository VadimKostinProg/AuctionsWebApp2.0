using BidMasterOnline.Domain.Enums;

namespace Feedbacks.Service.API.DTO.Participant
{
    public class SummarySupportTicketDTO
    {
        public long Id { get; set; }

        public required string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public SupportTicketStatus Status { get; set; }
    }
}
