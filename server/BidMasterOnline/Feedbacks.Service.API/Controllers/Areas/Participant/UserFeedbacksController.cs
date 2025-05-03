using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;
using Feedbacks.Service.API.ServiceContracts.Participant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacks.Service.API.Controllers.Areas.Participant
{
    [Route("api/participant/users")]
    [ApiController]
    [Authorize(Roles = UserRoles.Participant)]
    public class UserFeedbacksController : BaseController
    {
        private readonly IParticipantUserFeedbacksService _service;

        public UserFeedbacksController(IParticipantUserFeedbacksService service)
        {
            _service = service;
        }

        [HttpGet("{userId}/feedbacks")]
        public async Task<IActionResult> GetUserFeedbacks([FromRoute] long userId, 
            [FromQuery] PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<ParticipantUserFeedbackDTO>> result = 
                await _service.GetUserFeedbacksAsync(userId, pagination);

            return FromResult(result);
        }

        [HttpPost("feedbacks")]
        public async Task<IActionResult> PostUserFeedback([FromBody] ParticipantPostUserFeedbackDTO userFeedbackDTO)
        {
            ServiceResult result = await _service.PostUserFeedbackAsync(userFeedbackDTO);

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
