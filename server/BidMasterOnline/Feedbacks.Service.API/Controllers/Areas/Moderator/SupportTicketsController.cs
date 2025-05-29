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
        private readonly ISupportTicketsService _service;

        public SupportTicketsController(ISupportTicketsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetSupportTickets([FromQuery] SupportTicketsSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<SummarySupportTicketDTO>> result =
                await _service.GetSupportTicketsAsync(specifications);

            return FromResult(result);
        }

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetSupportTicketById([FromRoute] long ticketId)
        {
            ServiceResult<SupportTicketDTO> result = await _service.GetSupportTicketByIdAsync(ticketId);

            return FromResult(result);
        }

        [HttpPut("assing")]
        public async Task<IActionResult> AssingSupportTicket([FromBody] AssignSupportTicketDTO requestDTO)
        {
            ServiceResult result = await _service.AssignSupportTicketAsync(requestDTO);

            return FromResult(result);
        }

        [HttpPut("complete")]
        public async Task<IActionResult> CompleteSupportTicket([FromBody] CompleteSupportTicketDTO requestDTO)
        {
            ServiceResult result = await _service.CompleteSupportTicketAsync(requestDTO);

            return FromResult(result);
        }
    }
}
