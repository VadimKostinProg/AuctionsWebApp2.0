using BidMasterOnline.Core.Enums;
using BidMasterOnline.Domain.Enums;

namespace Auctions.Service.API.DTO.Moderator
{
    public class AuctionSpecificationsDTO
    {
        public int? AuctionId { get; set; }
        public int? CategoryId { get; set; }
        public int? TypeId { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public AuctionStatus? Status { get; set; }
        public string? SortBy { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.ASC;
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
