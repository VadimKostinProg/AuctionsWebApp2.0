using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.DTO.Moderator
{
    public class AuctionSummaryDTO : BaseDTO
    {
        public required string LotTitle { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime FinishTime { get; set; }

        public decimal CurrentPrice { get; set; }

        public double AverageScore { get; set; }

        public UserSummaryDTO? Auctionist { get; set; }
    }
}
