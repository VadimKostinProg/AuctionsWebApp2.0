using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;
using Feedbacks.Service.API.ServiceContracts.Moderator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacks.Service.API.Controllers.Areas.Moderator
{
    [Route("api/moderator/support-tickets")]
    [ApiController]
    [Authorize(Roles = UserRoles.Moderator)]
    public class SupportTicketsController : BaseController
    {
        private readonly IModeratorSupportTicketsService _service;

        public SupportTicketsController(IModeratorSupportTicketsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetSupportTickets([FromQuery] ModeratorSupportTicketsSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<ModeratorSummarySupportTicketDTO>> result =
                await _service.GetSupportTicketsAsync(specifications);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupportTicketById([FromRoute] long id)
        {
            ServiceResult<ModeratorSupportTicketDTO> result = await _service.GetSupportTicketByIdAsync(id);

            return FromResult(result);
        }

        [HttpPut("assing")]
        public async Task<IActionResult> AssingSupportTicket([FromBody] ModeratorAssignSupportTicketDTO requestDTO)
        {
            ServiceResult result = await _service.AssignSupportTicketAsync(requestDTO);

            return FromResult(result);
        }

        [HttpPut("complete")]
        public async Task<IActionResult> CompleteSupportTicket([FromBody] ModeratorCompleteSupportTicketDTO requestDTO)
        {
            ServiceResult result = await _service.CompleteSupportTicketAsync(requestDTO);

            return FromResult(result);
        }
    }
}
