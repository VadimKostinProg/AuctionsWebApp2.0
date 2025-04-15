using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class Role : EntityBase
    {
        [MaxLength(30)]
        public required string Name { get; set; }
    }
}
