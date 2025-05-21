using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.DTO.Moderator
{
    public class AuctionCategoryDTO : BaseDTO
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
