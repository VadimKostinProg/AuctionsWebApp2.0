using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.DTO.Moderator
{
    public class AuctionTypeDTO : BaseDTO
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
