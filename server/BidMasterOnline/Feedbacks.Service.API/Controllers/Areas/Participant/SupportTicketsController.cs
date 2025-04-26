using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;
using Feedbacks.Service.API.ServiceContracts.Participant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feedbacks.Service.API.Controllers.Areas.Participant
{
    [Route("api/participant/support-tickets")]
    [ApiController]
    [Authorize(Roles = UserRoles.Participant)]
    public class SupportTicketsController : BaseController
    {
        private readonly IParticipantSupportTicketsService _service;

        public SupportTicketsController(IParticipantSupportTicketsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserSupportTickets([FromQuery] PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<ParticipantSummarySupportTicketDTO>> result =
                await _service.GetUserSupportTicketsAsync(pagination);

            return FromResult(result);
        }

        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetUserSupportTickets([FromRoute] long ticketId)
        {
            ServiceResult<ParticipantSupportTicketDTO> result =
                await _service.GetSupportTicketByIdAsync(ticketId);

            return FromResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostSupportTicket([FromBody] ParticipantPostSupportTicketDTO ticketDTO)
        {
            ServiceResult result = await _service.PostSupportTicketAsync(ticketDTO);

            return FromResult(result);
        }
    }
}
