using System.ComponentModel.DataAnnotations;

namespace Feedbacks.Service.API.DTO.Participant
{
    public class PostSupportTicketDTO
    {
        [MaxLength(300)]
        public required string Title { get; set; }

        [MaxLength(10000)]
        public required string Text { get; set; }
    }
}
