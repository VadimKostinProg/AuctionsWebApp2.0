using BidMasterOnline.Application.Constants;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidMasterOnline.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetCommentsForAuction([FromQuery] Guid auctionId)
        {
            return Ok(await _commentsService.GetCommentsForAuctionAsync(auctionId));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult<CommentDTO>> GetCommentById([FromRoute] Guid id)
        {
            return Ok(await _commentsService.GetCommentByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult<string>> SetNewComment([FromBody] SetCommentDTO comment)
        {
            await _commentsService.SetNewCommentAsync(comment);

            return Ok(new { Message = "Comment has been set successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult<string>> DeleteComment([FromRoute] Guid id)
        {
            await _commentsService.DeleteCommentAsync(id);

            return Ok(new { Message = "Comment has been deleted successfully." });
        }

        [HttpDelete("own/{id}")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult<string>> DeleteOwnComment([FromRoute] Guid id)
        {
            await _commentsService.DeleteOwnCommentAsync(id);

            return Ok(new { Message = "Comment has been deleted successfully." });
        }
    }
}
