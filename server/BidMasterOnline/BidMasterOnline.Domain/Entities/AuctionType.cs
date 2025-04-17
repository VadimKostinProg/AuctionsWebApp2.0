using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class AuctionType : EntityBase
    {
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(1000)]
        public required string Description { get; set; }
    }
}
