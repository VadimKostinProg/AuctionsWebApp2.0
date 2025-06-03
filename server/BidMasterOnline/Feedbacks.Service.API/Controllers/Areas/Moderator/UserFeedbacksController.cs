using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;
using Feedbacks.Service.API.ServiceContracts.Moderator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacks.Service.API.Controllers.Areas.Moderator
{
    [Route("api/moderator/users")]
    [ApiController]
    [Authorize(Roles = UserRoles.Moderator)]
    public class UserFeedbacksController : BaseController
    {
        private readonly IUserFeedbacksService _service;

        public UserFeedbacksController(IUserFeedbacksService service)
        {
            _service = service;
        }

        [HttpGet("{userId}/feedbacks")]
        public async Task<IActionResult> GetUserFeedbacks([FromRoute] long userId,
            [FromQuery] PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<UserFeedbackDTO>> result =
                await _service.GetUserFeedbacksAsync(userId, pagination);

            return FromResult(result);
        }

        [HttpDelete("feedbacks/{feedbackId}")]
        public async Task<IActionResult> DeleteUserFeedback([FromRoute] long feedbackId)
        {
            ServiceResult result = await _service.DeleteUserFeedbackAsync(feedbackId);

            return FromResult(result);
        }
    }
}
