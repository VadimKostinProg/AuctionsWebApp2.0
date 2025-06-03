using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;
using Feedbacks.Service.API.ServiceContracts.Moderator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacks.Service.API.Controllers.Areas.Moderator
{
    [Route("api/moderator/auctions")]
    [ApiController]
    [Authorize(Roles = UserRoles.Moderator)]
    public class CommentsController : BaseController
    {
        private readonly IAuctionCommentsService _service;

        public CommentsController(IAuctionCommentsService service)
        {
            _service = service;
        }

        [HttpGet("{auctionId}/comments")]
        public async Task<IActionResult> GetAuctionComments([FromRoute] long auctionId,
            [FromQuery] PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<AuctionCommentDTO>> result = 
                await _service.GetAuctionCommentsAsync(auctionId, pagination);

            return FromResult(result);
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment([FromRoute] long commentId)
        {
            ServiceResult result =
                await _service.DeleteCommentAsync(commentId);

            return FromResult(result);
        }
    }
}
