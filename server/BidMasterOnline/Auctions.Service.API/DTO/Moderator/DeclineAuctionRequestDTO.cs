namespace Auctions.Service.API.DTO.Moderator
{
    public class DeclineAuctionRequestDTO
    {
        public long AuctionRequestId { get; set; }

        public required string Reason { get; set; }
    }
}
