using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class Category : EntityBase
    {
        [MaxLength(25)]
        public required string Name { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        public bool IsDeleted { get; set; }
    }
}
