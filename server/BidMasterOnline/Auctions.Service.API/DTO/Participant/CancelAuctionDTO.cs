namespace Auctions.Service.API.DTO.Participant
{
    public class CancelAuctionDTO
    {
        public long AuctionId { get; set; }
        
        public required string CancellationReason { get; set; }
    }
}
