namespace Auctions.Service.API.DTO.Moderator
{
    public class UpsertAuctionCategoryDTO
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
