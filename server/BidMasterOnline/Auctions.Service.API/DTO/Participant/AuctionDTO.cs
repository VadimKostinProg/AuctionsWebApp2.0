using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Auctions.Service.API.DTO.Participant
{
    public class AuctionDTO : AuctionSummaryDTO
    {
        public required string Category { get; set; }

        public required string Type { get; set; }

        public required string FinishMethod { get; set; }

        public required string LotDescription { get; set; }

        public TimeSpan AuctionTime { get; set; }

        public TimeSpan? FinishTimeInterval { get; set; }

        public decimal StartPrice { get; set; }

        public decimal BidAmountInterval { get; set; }

        public AuctionStatus Status { get; set; }

        public string? CancellationReason { get; set; }

        public decimal? FinishPrice { get; set; }

        public UserSummaryDTO? Winner { get; set; }
    }
}
