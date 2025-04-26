using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;

namespace Feedbacks.Service.API.ServiceContracts.Moderator
{
    public interface IModeratorSupportTicketsService
    {
        Task<ServiceResult<PaginatedList<ModeratorSummarySupportTicketDTO>>> GetSupportTicketsAsync(
            ModeratorSupportTicketsSpecificationsDTO specifications);

        Task<ServiceResult<ModeratorSupportTicketDTO>> GetSupportTicketByIdAsync(long supportTicketId);

        Task<ServiceResult> AssignSupportTicketAsync(ModeratorAssignSupportTicketDTO requestDTO);

        Task<ServiceResult> CompleteSupportTicketAsync(ModeratorCompleteSupportTicketDTO requestDTO);
    }
}
