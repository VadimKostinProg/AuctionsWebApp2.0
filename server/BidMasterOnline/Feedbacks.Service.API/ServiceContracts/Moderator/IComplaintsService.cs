using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;

namespace Feedbacks.Service.API.ServiceContracts.Moderator
{
    public interface IComplaintsService
    {
        Task<ServiceResult<PaginatedList<SummaryComplaintDTO>>> GetComplaintsAsync(
            ComplaintsSpecificationsDTO specifications);

        Task<ServiceResult<ComplaintDTO>> GetComplaintByIdAsync(long complaintId);

        Task<ServiceResult> AssignComplaintAsync(AssignComplaintDTO requestDTO);

        Task<ServiceResult> CompleteComplaintAsync(CompleteComplaintDTO requestDTO);
    }
}
