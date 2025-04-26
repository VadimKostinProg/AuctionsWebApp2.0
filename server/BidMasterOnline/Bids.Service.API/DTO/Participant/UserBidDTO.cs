namespace Bids.Service.API.DTO.Participant
{
    public class UserBidDTO
    {
        public long AuctionId { get; set; }

        public long BidderId { get; set; }

        public decimal Amount { get; set; }

        public DateTime Time { get; set; }

        public bool Deleted { get; set; }

        public string AuctionName { get; set; } = string.Empty;
    }
}
