using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Bids.Service.API.DTO.Participant;
using Bids.Service.API.ServiceContracts.Participant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bids.Service.API.Controllers.Areas.Participant
{
    [Route("api/participant/auctions")]
    [ApiController]
    [Authorize(Roles = UserRoles.Participant)]
    public class BidsController : BaseController
    {
        private readonly IBidsService _service;

        public BidsController(IBidsService service)
        {
            _service = service;
        }

        [HttpGet("bids/own")]
        public async Task<IActionResult> GetUserBids([FromQuery] PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<UserBidDTO>> result = await _service.GetUserBidsAsync(pagination);

            return FromResult(result);
        }

        [HttpGet("{auctionId}/bids")]
        public async Task<IActionResult> GetAuctionBids([FromRoute] long auctionId,
            [FromQuery] PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<AuctionBidDTO>> result = await _service.GetAuctionBidsAsync(auctionId,
                pagination);

            return FromResult(result);
        }

        [HttpPost("bids")]
        public async Task<IActionResult> PostBid([FromBody] PostBidDTO bidDTO)
        {
            ServiceResult result = await _service.PostBidOnAuctionAsync(bidDTO);

            return FromResult(result);
        }
    }
}
