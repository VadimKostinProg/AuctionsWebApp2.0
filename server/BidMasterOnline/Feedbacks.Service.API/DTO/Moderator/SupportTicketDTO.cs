using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class SupportTicketDTO : BaseDTO
    {
        public long UserId { get; set; }

        public long? ModeratorId { get; set; }

        public required string Title { get; set; }

        public required string Text { get; set; }

        public SupportTicketStatus Status { get; set; }

        public string? ModeratorComment { get; set; }

        public string SubmitUsername { get; set; } = string.Empty;

        public string? ModeratorName { get; set; }
    }
}
