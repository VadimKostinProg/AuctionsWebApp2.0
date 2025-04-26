using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;

namespace Feedbacks.Service.API.ServiceContracts.Participant
{
    public interface IParticipantSupportTicketsService
    {
        Task<ServiceResult> PostSupportTicketAsync(ParticipantPostSupportTicketDTO ticketDTO);

        Task<ServiceResult<PaginatedList<ParticipantSummarySupportTicketDTO>>> GetUserSupportTicketsAsync(
            PaginationRequestDTO pagination);

        Task<ServiceResult<ParticipantSupportTicketDTO>> GetSupportTicketByIdAsync(long ticketId);
    }
}
