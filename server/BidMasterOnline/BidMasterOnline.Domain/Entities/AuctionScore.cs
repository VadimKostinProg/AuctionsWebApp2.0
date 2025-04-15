using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class AuctionScore : EntityBase
    {
        public Guid AuctionId { get; set; }

        public Guid UserId { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }
    }
}
