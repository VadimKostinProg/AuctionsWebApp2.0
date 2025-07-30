using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Auctions.Service.API.DTO.Participant
{
    public class AuctionSummaryDTO
    {
        public long Id { get; set; }

        public required string LotTitle { get; set; }

        public required string Category { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset FinishTime { get; set; }

        public decimal StartPrice { get; set; }

        public decimal CurrentPrice { get; set; }

        public double? AverageScore { get; set; }

        public AuctionStatus Status { get; set; }

        public UserSummaryDTO? Auctioneer { get; set; }

        public List<string> ImageUrls { get; set; } = [];
    }
}
