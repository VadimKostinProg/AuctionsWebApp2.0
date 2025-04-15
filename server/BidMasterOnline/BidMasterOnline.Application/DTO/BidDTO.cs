namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for the bid of auction. (RESPONSE)
    /// </summary>
    public class BidDTO
    {
        public Guid Id { get; set; }
        public Guid AuctionId { get; set; }
        public string AuctionName { get; set; }
        public Guid BidderId { get; set; }
        public string BidderUsername { get; set; }
        public string DateAndTime { get; set; }
        public decimal Amount { get; set; }
        public bool IsWinning { get; set; }
    }
}
