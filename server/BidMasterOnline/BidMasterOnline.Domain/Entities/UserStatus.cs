using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class UserStatus : EntityBase
    {
        [MaxLength(10)]
        public required string Name { get; set; }
    }
}
