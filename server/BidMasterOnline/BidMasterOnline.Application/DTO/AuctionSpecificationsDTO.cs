using BidMasterOnline.Application.Enums;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for acution specifications for filtering, sorting and pagination. (REQUEST)
    /// </summary>
    public class AuctionSpecificationsDTO : SpecificationsDTO
    {
        public Guid? CategoryId { get; set; }
        public Guid? AuctionistId { get; set; }
        public Guid? WinnerId { get; set; }
        public decimal? MinStartPrice { get; set; }
        public decimal? MaxStartPrice { get; set; }
        public decimal? MinCurrentBid { get; set; }
        public decimal? MaxCurrentBid { get; set; }
        public AuctionStatus? Status { get; set; }
    }
}
