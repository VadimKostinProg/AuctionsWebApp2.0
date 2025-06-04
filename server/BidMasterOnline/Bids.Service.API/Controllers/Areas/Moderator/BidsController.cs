using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Bids.Service.API.DTO.Moderator;
using Bids.Service.API.ServiceContracts.Moderator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bids.Service.API.Controllers.Areas.Moderator
{
    [Route("api/moderator")]
    [ApiController]
    [Authorize(Roles = UserRoles.Moderator)]
    public class BidsController : BaseController
    {
        private readonly IBidsService _service;

        public BidsController(IBidsService service)
        {
            _service = service;
        }

        [HttpGet("users/{userId}/bids")]
        public async Task<IActionResult> GetUserBids([FromRoute] long userId)
        {
            ServiceResult<PaginatedList<UserBidDTO>> result = await _service.GetUserBidsAsync(userId);

            return FromResult(result);
        }

        [HttpGet("auctions/{auctionId}/bids")]
        public async Task<IActionResult> GetAuctionBids([FromRoute] long auctionId, [FromQuery] PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<AuctionBidDTO>> result = await _service.GetAuctionBidsAsync(auctionId, pagination);

            return FromResult(result);
        }
    }
}
