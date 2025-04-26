using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;

namespace Feedbacks.Service.API.ServiceContracts.Participant
{
    public interface IParticipantUserFeedbacksService
    {
        Task<ServiceResult> PostUserFeedbackAsync(ParticipantPostUserFeedbackDTO userFeedbackDTO);

        Task<ServiceResult<PaginatedList<ParticipantUserFeedbackDTO>>> GetUserFeedbacksAsync(long userId,
            PaginationRequestDTO pagination);
    }
}
