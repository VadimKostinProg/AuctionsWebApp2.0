using System.ComponentModel.DataAnnotations.Schema;

namespace BidMasterOnline.Domain.Entities
{
    public class Bid : EntityBase
    {
        public Guid AuctionId { get; set; }

        public Guid BidderId { get; set; }

        public DateTime DateAndTime { get; set; }

        public decimal Amount { get; set; }

        public bool IsWinning { get; set; }

        [ForeignKey(nameof(AuctionId))]
        public virtual Auction? Auction { get; set; }

        [ForeignKey(nameof(BidderId))]
        public virtual User? Bidder { get; set; }
    }
}
