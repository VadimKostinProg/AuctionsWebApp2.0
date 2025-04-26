using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;
using Feedbacks.Service.API.ServiceContracts.Participant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacks.Service.API.Controllers.Areas.Participant
{
    [Route("api/participant/auctions")]
    [ApiController]
    [Authorize(Roles = UserRoles.Participant)]
    public class CommentsController : BaseController
    {
        private readonly IParticipantAuctionCommentsService _service;

        public CommentsController(IParticipantAuctionCommentsService participantAuctionCommentsService)
        {
            _service = participantAuctionCommentsService;
        }

        [HttpGet("{auctionId}/comments")]
        public async Task<IActionResult> GetAuctionComments([FromRoute] long auctionId, 
            [FromQuery] PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<ParticipantAuctionCommentDTO>> result = 
                await _service.GetAuctionCommentsAsync(auctionId, pagination);

            return FromResult(result);
        }

        [HttpPost("comments")]
        public async Task<IActionResult> PostAuctionComment([FromBody] ParticipantPostCommentDTO commentDTO)
        {
            ServiceResult result = await _service.PostAuctionCommentAsync(commentDTO);

            return FromResult(result);
        }
    }
}
