using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;

namespace Feedbacks.Service.API.ServiceContracts.Participant
{
    public interface IUserFeedbacksService
    {
        Task<ServiceResult> PostUserFeedbackAsync(PostUserFeedbackDTO userFeedbackDTO);

        Task<ServiceResult<PaginatedList<UserFeedbackDTO>>> GetUserFeedbacksAsync(long userId,
            PaginationRequestDTO pagination);

        Task<ServiceResult> DeleteUserFeedbackAsync(long userFeedbackId);
    }
}
