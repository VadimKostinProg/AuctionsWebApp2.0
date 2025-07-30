using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.DTO.Moderator
{
    public class AuctionRequestDTO : AuctionRequestSummaryDTO
    {
        public required string LotDescription { get; set; }

        public required string Category { get; set; }

        public required string Type { get; set; }

        public required string FinishMethod { get; set; }

        public DateTime? RequestedStartTime { get; set; }

        public TimeSpan? FinishTimeInterval { get; set; }

        public decimal BidAmountInterval { get; set; }

        public decimal? AimPrice { get; set; }

        public string? ReasonDeclined { get; set; }

        public UserSummaryDTO? RequestedByUser { get; set; }

        public List<string> ImageUrls { get; set; } = [];
    }
}
