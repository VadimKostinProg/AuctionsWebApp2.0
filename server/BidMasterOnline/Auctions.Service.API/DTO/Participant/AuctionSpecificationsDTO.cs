using BidMasterOnline.Core.Enums;
using BidMasterOnline.Domain.Enums;

namespace Auctions.Service.API.DTO.Participant
{
    public class AuctionSpecificationsDTO
    {
        public string? SearchTerm { get; set; }
        public long? CategoryId { get; set; }
        public long? TypeId { get; set; }
        public decimal? MinStartPrice { get; set; }
        public decimal? MaxStartPrice { get; set; }
        public decimal? MinCurrentPrice { get; set; }
        public decimal? MaxCurrentPrice { get; set; }
        public AuctionStatus? AuctionStatus { get; set; }
        public string? SortBy { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.ASC;
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
