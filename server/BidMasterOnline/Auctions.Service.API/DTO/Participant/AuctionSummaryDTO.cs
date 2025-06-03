using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.DTO.Participant
{
    public class AuctionSummaryDTO
    {
        public long Id { get; set; }

        public required string LotTitle { get; set; }

        public required string Category { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime FinishTime { get; set; }

        public decimal StartPrice { get; set; }

        public decimal CurrentPrice { get; set; }

        public double? AverageScore { get; set; }

        public UserSummaryDTO? Auctionist { get; set; }

        public List<string> ImageUrls { get; set; } = [];
    }
}
