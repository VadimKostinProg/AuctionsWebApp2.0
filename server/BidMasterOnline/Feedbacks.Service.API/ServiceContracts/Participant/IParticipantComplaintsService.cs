using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;

namespace Feedbacks.Service.API.ServiceContracts.Participant
{
    public interface IParticipantComplaintsService
    {
        Task<ServiceResult> PostComplaintAsync(ParticipantPostComplaintDTO complaintDTO);

        Task<ServiceResult<PaginatedList<ParticipantSummaryComplaintDTO>>> GetUserComplaintsAsync(
            PaginationRequestDTO pagination);
        
        Task<ServiceResult<ParticipantComplaintDTO>> GetComplaintByIdAsync(long complaintId);
    }
}
