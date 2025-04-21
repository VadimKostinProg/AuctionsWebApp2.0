using BidMasterOnline.Domain.Enums;

namespace Auctions.Service.API.DTO
{
    public class AuctionRequestSummaryDTO
    {
        public long Id { get; set; }

        public required string LotTitle { get; set; }

        public TimeSpan RequestedAuctionTime { get; set; }

        public decimal StartPrice { get; set; }

        public AuctionRequestStatus Status { get; set; }

        public List<AuctionImageDTO> Images { get; set; } = [];
    }
}
