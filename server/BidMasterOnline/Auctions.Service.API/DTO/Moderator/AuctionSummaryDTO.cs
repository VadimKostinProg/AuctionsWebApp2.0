using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Auctions.Service.API.DTO.Moderator
{
    public class AuctionSummaryDTO : BaseDTO
    {
        public required string Category { get; set; }

        public required string Type { get; set; }

        public required string LotTitle { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime FinishTime { get; set; }

        public decimal StartPrice { get; set; }

        public decimal CurrentPrice { get; set; }

        public AuctionStatus Status { get; set; }
    }
}
