using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;
using Feedbacks.Service.API.ServiceContracts.Moderator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacks.Service.API.Controllers.Areas.Moderator
{
    [Route("api/moderator/complaints")]
    [ApiController]
    [Authorize(Roles = UserRoles.Moderator)]
    public class ComplaintsController : BaseController
    {
        private readonly IModeratorComplaintsService _service;

        public ComplaintsController(IModeratorComplaintsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetComplaints([FromQuery] ModeratorComplaintsSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<ModeratorSummaryComplaintDTO>> result =
                await _service.GetComplaintsAsync(specifications);

            return FromResult(result);
        }

        [HttpGet("{complaintId}")]
        public async Task<IActionResult> GetComplaintById([FromRoute] long complaintId)
        {
            ServiceResult<ModeratorComplaintDTO> result = await _service.GetComplaintByIdAsync(complaintId);

            return FromResult(result);
        }

        [HttpPut("assing")]
        public async Task<IActionResult> AssingComplaint([FromBody] ModeratorAssignComplaintDTO requestDTO)
        {
            ServiceResult result = await _service.AssignComplaintAsync(requestDTO);

            return FromResult(result);
        }

        [HttpPut("complete")]
        public async Task<IActionResult> CompleteComplaint([FromBody] ModeratorCompleteComplaintDTO requestDTO)
        {
            ServiceResult result = await _service.CompleteComplaintAsync(requestDTO);

            return FromResult(result);
        }
    }
}
