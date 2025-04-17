using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public abstract class EntityBase
    {
        [Key]
        public long Id { get; set; }

        public bool Deleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? ModifiedAt { get; set; }

        public string? ModifiedBy { get; set; }
    }
}