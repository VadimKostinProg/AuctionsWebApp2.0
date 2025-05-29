using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;

namespace Feedbacks.Service.API.ServiceContracts.Participant
{
    public interface ISupportTicketsService
    {
        Task<ServiceResult> PostSupportTicketAsync(PostSupportTicketDTO ticketDTO);

        Task<ServiceResult<PaginatedList<SummarySupportTicketDTO>>> GetUserSupportTicketsAsync(
            PaginationRequestDTO pagination);

        Task<ServiceResult<SupportTicketDTO>> GetSupportTicketByIdAsync(long ticketId);
    }
}
