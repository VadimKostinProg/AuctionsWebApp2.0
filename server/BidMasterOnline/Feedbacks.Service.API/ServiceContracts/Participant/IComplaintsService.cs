using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;

namespace Feedbacks.Service.API.ServiceContracts.Participant
{
    public interface IComplaintsService
    {
        Task<ServiceResult> PostComplaintAsync(PostComplaintDTO complaintDTO);

        Task<ServiceResult<PaginatedList<SummaryComplaintDTO>>> GetUserComplaintsAsync(
            PaginationRequestDTO pagination);
        
        Task<ServiceResult<ComplaintDTO>> GetComplaintByIdAsync(long complaintId);
    }
}
