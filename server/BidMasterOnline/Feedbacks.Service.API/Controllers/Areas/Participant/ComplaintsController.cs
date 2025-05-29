using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;
using Feedbacks.Service.API.ServiceContracts.Participant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacks.Service.API.Controllers.Areas.Participant
{
    [Route("api/participant/complaints")]
    [ApiController]
    [Authorize(Roles = UserRoles.Participant)]
    public class ComplaintsController : BaseController
    {
        private readonly IComplaintsService _service;

        public ComplaintsController(IComplaintsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserComplaints([FromQuery] PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<SummaryComplaintDTO>> result = 
                await _service.GetUserComplaintsAsync(pagination);

            return FromResult(result);
        }

        [HttpGet("{complaintId}")]
        public async Task<IActionResult> GetComplaintById([FromRoute] long complaintId)
        {
            ServiceResult<ComplaintDTO> result =
                await _service.GetComplaintByIdAsync(complaintId);

            return FromResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostComplaint([FromBody] PostComplaintDTO complaintDTO)
        {
            ServiceResult result = await _service.PostComplaintAsync(complaintDTO);

            return FromResult(result);
        }
    }
}
