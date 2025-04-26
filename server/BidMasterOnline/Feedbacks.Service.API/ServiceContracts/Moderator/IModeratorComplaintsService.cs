using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;

namespace Feedbacks.Service.API.ServiceContracts.Moderator
{
    public interface IModeratorComplaintsService
    {
        Task<ServiceResult<PaginatedList<ModeratorSummaryComplaintDTO>>> GetComplaintsAsync(
            ModeratorComplaintsSpecificationsDTO specifications);

        Task<ServiceResult<ModeratorComplaintDTO>> GetComplaintByIdAsync(long complaintId);

        Task<ServiceResult> AssignComplaintAsync(ModeratorAssignComplaintDTO requestDTO);

        Task<ServiceResult> CompleteComplaintAsync(ModeratorCompleteComplaintDTO requestDTO);
    }
}
