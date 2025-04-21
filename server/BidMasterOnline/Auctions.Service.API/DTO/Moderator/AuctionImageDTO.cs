using BidMasterOnline.Core.DTO;

namespace Auctions.Service.API.DTO.Moderator
{
    public class AuctionImageDTO : BaseDTO
    {
        public required string Url { get; set; }

        public required string PublicId { get; set; }
    }
}
