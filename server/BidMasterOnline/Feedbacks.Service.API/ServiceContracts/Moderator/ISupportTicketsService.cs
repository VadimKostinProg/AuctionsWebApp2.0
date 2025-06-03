using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;

namespace Feedbacks.Service.API.ServiceContracts.Moderator
{
    public interface ISupportTicketsService
    {
        Task<ServiceResult<PaginatedList<SummarySupportTicketDTO>>> GetSupportTicketsAsync(
            SupportTicketsSpecificationsDTO specifications);

        Task<ServiceResult<SupportTicketDTO>> GetSupportTicketByIdAsync(long supportTicketId);

        Task<ServiceResult> AssignSupportTicketAsync(AssignSupportTicketDTO requestDTO);

        Task<ServiceResult> CompleteSupportTicketAsync(CompleteSupportTicketDTO requestDTO);
    }
}
