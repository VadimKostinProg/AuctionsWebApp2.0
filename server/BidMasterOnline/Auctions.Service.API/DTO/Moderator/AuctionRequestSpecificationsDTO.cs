using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Auctions.Service.API.DTO.Moderator
{
    public class AuctionRequestSpecificationsDTO : PaginationRequestDTO
    {
        public string? SearchTerm { get; set; }

        public AuctionRequestStatus Status { get; set; }
    }
}
