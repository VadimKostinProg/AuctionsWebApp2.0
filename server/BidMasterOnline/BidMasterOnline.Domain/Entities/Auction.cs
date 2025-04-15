using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BidMasterOnline.Domain.Entities
{
    public class Auction : EntityBase
    {
        [MaxLength(50)]
        public required string Name { get; set; }

        public Guid AuctionistId { get; set; }

        public Guid CategoryId { get; set; }

        [MaxLength(1000)]
        public required string LotDescription { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime FinishDateTime { get; set; }

        public long AuctionTime { get; set; }

        public long? FinishInterval { get; set; }

        public decimal StartPrice { get; set; }

        public Guid FinishTypeId { get; set; }

        public Guid StatusId { get; set; }

        public bool IsApproved { get; set; }

        [ForeignKey(nameof(AuctionistId))]
        public User? Auctionist { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        [ForeignKey(nameof(FinishTypeId))]
        public AuctionFinishType? FinishType { get; set; }

        [ForeignKey(nameof(StatusId))]
        public AuctionStatus? Status { get; set; }

        public AuctionPaymentDeliveryOptions? PaymentDeliveryOptions { get; set; }

        public ICollection<AuctionImage>? Images { get; set; }

        public ICollection<Bid>? Bids { get; set; }

        public ICollection<AuctionScore>? Scores { get; set; }
    }
}
