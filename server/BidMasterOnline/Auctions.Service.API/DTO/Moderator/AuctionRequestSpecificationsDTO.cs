using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Enums;
using BidMasterOnline.Domain.Enums;

namespace Auctions.Service.API.DTO.Moderator
{
    public class AuctionRequestSpecificationsDTO : PaginationRequestDTO
    {
        public AuctionRequestStatus Status { get; set; }

        public SortDirection SortDirection { get; set; }
    }
}
