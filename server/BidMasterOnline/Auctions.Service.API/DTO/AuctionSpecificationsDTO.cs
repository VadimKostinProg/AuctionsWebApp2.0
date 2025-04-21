using BidMasterOnline.Domain.Enums;

namespace Auctions.Service.API.DTO
{
    public class AuctionSpecificationsDTO
    {
        public string? SearchTerm { get; set; }
        public long? CategoryId { get; set; }
        public long? AuctionistId { get; set; }
        public long? WinnerId { get; set; }
        public decimal? MinStartPrice { get; set; }
        public decimal? MaxStartPrice { get; set; }
        public decimal? MinCurrentBid { get; set; }
        public decimal? MaxCurrentBid { get; set; }
        public AuctionStatus? Status { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
