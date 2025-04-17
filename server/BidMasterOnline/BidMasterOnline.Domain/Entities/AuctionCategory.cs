using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class AuctionCategory : EntityBase
    {
        [MaxLength(50)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public required string Description { get; set; }
    }
}
