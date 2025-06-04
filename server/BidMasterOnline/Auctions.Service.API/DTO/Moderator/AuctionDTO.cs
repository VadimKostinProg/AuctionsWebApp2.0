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

        public UserSummaryDTO? Auctionist { get; set; }

        public UserSummaryDTO? Winner { get; set; }

        public List<string> ImageUrls { get; set; } = [];
    }
}
