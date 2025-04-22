namespace Auctions.Service.API.DTO.Moderator
{
    public class CancelAuctionDTO
    {
        public long AuctionId { get; set; }
        
        public required string Reason { get; set; }
    }
}
