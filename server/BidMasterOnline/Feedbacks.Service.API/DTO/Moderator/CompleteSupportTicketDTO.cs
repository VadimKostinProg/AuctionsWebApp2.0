using System.ComponentModel.DataAnnotations;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class CompleteSupportTicketDTO
    {
        public long SupportTicketId { get; set; }

        [MaxLength(10000)]
        public required string ModeratorComment { get; set; }
    }
}
