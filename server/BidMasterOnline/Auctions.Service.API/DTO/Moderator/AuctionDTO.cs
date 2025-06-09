using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Auctions.Service.API.DTO.Moderator
{
    public class AuctionDTO : AuctionSummaryDTO
    {
        public required string FinishMethod { get; set; }

        public required string LotDescription { get; set; }

        public TimeSpan AuctionTime { get; set; }

        public TimeSpan? FinishTimeInterval { get; set; }

        public decimal BidAmountInterval { get; set; }

        public string? CancellationReason { get; set; }

        public double? AverageScore { get; set; }

        public bool IsPaymentPerformed { get; set; }

        public DateTime? PaymentPerformedTime { get; set; }

        public bool IsDeliveryPerformed { get; set; }

        public DateTime? DeliveryPerformedTime { get; set; }

        public string? DeliveryWaybill { get; set; }

        public UserSummaryDTO? Auctioneer { get; set; }

        public UserSummaryDTO? Winner { get; set; }

        public List<string> ImageUrls { get; set; } = [];
    }
}
