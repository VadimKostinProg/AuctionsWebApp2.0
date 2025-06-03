using BidMasterOnline.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Models.Entities
{
    public class AuctionRequest : EntityBase
    {
        public long AuctionCategoryId { get; set; }

        public long AuctionTypeId { get; set; }

        public long AuctionFinishMethodId { get; set; }

        public long RequestedByUserId { get; set; }

        [MaxLength(500)]
        public required string LotTitle { get; set; }

        [MaxLength(5000)]
        public required string LotDescription { get; set; }

        // ASAP if null
        public DateTime? RequestedStartTime { get; set; }

        public long RequestedAuctionTimeInTicks { get; set; }

        public decimal StartPrice { get; set; }

        public long? FinishTimeIntervalInTicks { get; set; }

        public decimal BidAmountInterval { get; set; }

        public decimal? AimPrice { get; set; }

        public AuctionRequestStatus Status { get; set; }

        public string? ReasonDeclined { get; set; }

        public User? RequestedByUser { get; set; }

        public AuctionCategory? Category { get; set; }

        public AuctionType? Type { get; set; }

        public AuctionFinishMethod? FinishMethod { get; set; }

        public ICollection<AuctionImage>? Images { get; set; }
    }
}
