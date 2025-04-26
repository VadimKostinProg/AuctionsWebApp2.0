using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;
using Feedbacks.Service.API.ServiceContracts.Participant;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacks.Service.API.Controllers.Areas.Participant
{
    [Route("api/participant/complaints")]
    [ApiController]
    public class ComplaintsController : BaseController
    {
        private readonly IParticipantComplaintsService _service;

        public ComplaintsController(IParticipantComplaintsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserComplaints([FromQuery] PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<ParticipantSummaryComplaintDTO>> result = 
                await _service.GetUserComplaintsAsync(pagination);

            return FromResult(result);
        }

        [HttpGet("{complaintId}")]
        public async Task<IActionResult> GetUserComplaints([FromRoute] long complaintId)
        {
            ServiceResult<ParticipantComplaintDTO> result =
                await _service.GetComplaintByIdAsync(complaintId);

            return FromResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostComplaint([FromBody] ParticipantPostComplaintDTO complaintDTO)
        {
            ServiceResult result = await _service.PostComplaintAsync(complaintDTO);

            return FromResult(result);
        }
    }
}
