using BidMasterOnline.Application.Constants;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidMasterOnline.API.Controllers
{
    [Route("api/v{version:apiVersion}/technical-support")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TechnicalSupportController : ControllerBase
    {
        private readonly ITechnicalSupportRequestsService _technicalSupportRequestsService;
        private readonly IComplaintsService _complaintsService;

        public TechnicalSupportController(ITechnicalSupportRequestsService technicalSupportService, IComplaintsService complaintsService)
        {
            _technicalSupportRequestsService = technicalSupportService;
            _complaintsService = complaintsService;
        }

        #region TECHNICAL SUPPORT REQUESTS
        [HttpGet("requests/list")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult<ListModel<TechnicalSupportRequestDTO>>> GetTechnicalSupportRequestsList(
            [FromQuery] TechnicalSupportRequestSpecificationsDTO specifications)
        {
            return Ok(await _technicalSupportRequestsService.GetTechnicalSupportRequestsListAsync(specifications));
        }

        [HttpGet("requests/{id}")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult<TechnicalSupportRequestDTO>> GetTechnicalSupportRequestById(
            [FromRoute] Guid id)
        {
            return Ok(await _technicalSupportRequestsService.GetTechnicalSupportRequestByIdAsync(id));
        }

        [HttpPost("requests")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult<string>> SetTechnicalSupportRequest(
            [FromBody] SetTechnicalSupportRequestDTO technicalSupportRequest)
        {
            await _technicalSupportRequestsService.SetTechnicalSupportRequestAsync(technicalSupportRequest);

            return Ok(new { Message = "Your request has been successfully sent to the technical support team." });
        }

        [HttpPut("requests/{id}")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult<string>> HandleTechnicalSupportRequest(
            [FromRoute] Guid id)
        {
            await _technicalSupportRequestsService.HandleTechnicalSupportRequestAsync(id);

            return Ok(new { Message = "Technical support request is handled successfully." });
        }
        #endregion

        #region COMPLAINTS
        [HttpGet("complaints/list")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult<ListModel<ComplaintDTO>>> GetComplaintsList(
            [FromQuery] ComplaintSpecificationsDTO specifications)
        {
            return Ok(await _complaintsService.GetComplaintsListAsync(specifications));
        }

        [HttpGet("complaints/{id}")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult<ComplaintDTO>> GetComplaintById(
            [FromRoute] Guid id)
        {
            return Ok(await _complaintsService.GetComplaintByIdAsync(id));
        }

        [HttpPost("complaints")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult<string>> SetComplaint([FromBody] SetComplaintDTO complaint)
        {
            await _complaintsService.SetNewComplaintAsync(complaint);

            return Ok(new { Message = "Your complaint has been successfully sent to the technical support team." });
        }

        [HttpPut("complaints/{id}")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult<string>> HandleComplaint([FromRoute] Guid id)
        {
            await _complaintsService.HandleComplaintAsync(id);

            return Ok(new { Message = "Complaint is successfully handled." });
        }
        #endregion
    }
}
