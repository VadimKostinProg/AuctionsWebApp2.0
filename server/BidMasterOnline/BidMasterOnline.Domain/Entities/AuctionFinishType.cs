using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class AuctionFinishType : EntityBase
    {
        
        [MaxLength(50)]
        public required string Name { get; set; }

        
        [MaxLength(300)]
        public required string Description { get; set; }
    }
}
