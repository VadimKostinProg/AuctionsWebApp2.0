using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Auctions.Service.API.DTO.Moderator
{
    public class AuctionRequestSummaryDTO : BaseDTO
    {
        public required string LotTitle { get; set; }

        public TimeSpan RequestedAuctionTime { get; set; }

        public decimal StartPrice { get; set; }

        public AuctionRequestStatus Status { get; set; }
    }
}
