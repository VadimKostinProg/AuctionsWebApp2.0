using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Models.Entities
{
    public class Role : EntityBase
    {
        [MaxLength(30)]
        public required string Name { get; set; }
    }
}
