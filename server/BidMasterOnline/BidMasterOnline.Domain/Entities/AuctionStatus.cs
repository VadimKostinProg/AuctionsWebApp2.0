using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class AuctionStatus : EntityBase
    {
        [MaxLength(10)]
        public required string Name { get; set; }
    }
}
