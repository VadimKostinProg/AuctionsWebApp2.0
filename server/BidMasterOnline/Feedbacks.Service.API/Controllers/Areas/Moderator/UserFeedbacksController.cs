using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;
using Feedbacks.Service.API.ServiceContracts.Moderator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacks.Service.API.Controllers.Areas.Moderator
{
    [Route("api/moderator")]
    [ApiController]
    [Authorize(Roles = UserRoles.Moderator)]
    public class UserFeedbacksController : BaseController
    {
        private readonly IModeratorUserFeedbacksService _service;

        public UserFeedbacksController(IModeratorUserFeedbacksService service)
        {
            _service = service;
        }

        [HttpGet("{userId}/feedbacks")]
        public async Task<IActionResult> GetUserFeedbacks([FromRoute] long userId,
            [FromQuery] PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<ModeratorUserFeedbackDTO>> result =
                await _service.GetUserFeedbacksAsync(userId, pagination);

            return FromResult(result);
        }

        [HttpDelete("feedbacks/{id}")]
        public async Task<IActionResult> DeleteUserFeedback([FromRoute] long id)
        {
            ServiceResult result = await _service.DeleteUserFeedbackAsync(id);

            return FromResult(result);
        }
    }
}
