using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;

namespace Feedbacks.Service.API.ServiceContracts.Moderator
{
    public interface IUserFeedbacksService
    {
        Task<ServiceResult<PaginatedList<UserFeedbackDTO>>> GetUserFeedbacksAsync(long userId, 
            PaginationRequestDTO pagination);

        Task<ServiceResult> DeleteUserFeedbackAsync(long userFeedbackId);
    }
}
