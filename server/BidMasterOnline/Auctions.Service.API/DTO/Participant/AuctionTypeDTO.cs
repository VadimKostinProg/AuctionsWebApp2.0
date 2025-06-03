namespace Auctions.Service.API.DTO.Participant
{
    public class AuctionTypeDTO
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
