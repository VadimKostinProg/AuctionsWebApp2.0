using BidMasterOnline.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Models.Entities
{
    public class Auction : EntityBase
    {
        public long AuctionCategoryId { get; set; }

        public long AuctionTypeId { get; set; }

        public long AuctionFinishMethodId { get; set; }

        public long AuctioneerId { get; set; }

        public long? WinnerId { get; set; }

        [MaxLength(500)]
        public required string LotTitle { get; set; }

        [MaxLength(5000)]
        public required string LotDescription { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime FinishTime { get; set; }

        public long AuctionTimeInTicks { get; set; }

        public long? FinishTimeIntervalInTicks { get; set; }

        public decimal BidAmountInterval { get; set; }

        public decimal StartPrice { get; set; }

        public decimal CurrentPrice { get; set; }

        public decimal? AimPrice { get; set; }

        public double? AverageScore { get; set; }

        public AuctionStatus Status { get; set; }

        public string? CancellationReason { get; set; }

        public decimal? FinishPrice { get; set; }

        public bool IsPaymentPerformed { get; set; }

        public DateTime? PaymentPerformedTime { get; set; }

        public bool IsDeliveryPerformed { get; set; }

        public DateTime? DeliveryPerformedTime { get; set; }

        public string? DeliveryWaybill { get; set; }

        public User? Auctioneer { get; set; }

        public User? Winner { get; set; }

        public AuctionCategory? Category { get; set; }

        public AuctionType? Type { get; set; }

        public AuctionFinishMethod? FinishMethod { get; set; }

        public ICollection<AuctionImage>? Images { get; set; }

        public ICollection<Bid>? Bids { get; set; }
    }
}
