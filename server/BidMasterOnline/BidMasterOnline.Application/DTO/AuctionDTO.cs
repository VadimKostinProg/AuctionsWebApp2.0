namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO of auction. (RESPONSE)
    /// </summary>
    public class AuctionDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public Guid AuctionistId { get; set; }
        public string Auctionist { get; set; }
        public string AuctionTime { get; set; }
        public string FinishDateAndTime { get; set; }
        public decimal StartPrice { get; set; }
        public decimal CurrentBid { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
