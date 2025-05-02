using BidMasterOnline.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Models.Entities
{
    public class SupportTicket : EntityBase
    {
        public long UserId { get; set; }

        public long? ModeratorId { get; set; }

        [MaxLength(300)]
        public required string Title { get; set; }

        [MaxLength(10000)]
        public required string Text { get; set; }

        public SupportTicketStatus Status { get; set; }

        [MaxLength(10000)]
        public string? ModeratorComment { get; set; }

        public User? User { get; set; }

        public User? Moderator { get; set; }
    }
}
