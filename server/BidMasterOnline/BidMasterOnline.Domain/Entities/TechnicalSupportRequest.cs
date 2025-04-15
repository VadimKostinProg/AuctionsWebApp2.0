using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class TechnicalSupportRequest : EntityBase
    {
        public Guid UserId { get; set; }

        [MaxLength(1000)]
        public required string RequestText { get; set; }

        public DateTime DateAndTime { get; set; }

        public bool IsHandled { get; set; }

        public User User { get; set; }
    }
}
