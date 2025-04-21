using BidMasterOnline.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Models.Entities
{
    public class TechnicalSupportRequest : EntityBase
    {
        public long UserId { get; set; }

        public long? ModeratorId { get; set; }

        [MaxLength(1000)]
        public required string Text { get; set; }

        public TechnicalSupportRequestState State { get; set; }

        public User? User { get; set; }

        public User? Moderator { get; set; }
    }
}
