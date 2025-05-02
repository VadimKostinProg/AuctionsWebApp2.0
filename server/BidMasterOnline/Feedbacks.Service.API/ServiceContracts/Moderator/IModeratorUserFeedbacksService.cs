using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;

namespace Feedbacks.Service.API.ServiceContracts.Moderator
{
    public interface IModeratorUserFeedbacksService
    {
        Task<ServiceResult<PaginatedList<ModeratorUserFeedbackDTO>>> GetUserFeedbacksAsync(long userId, 
            PaginationRequestDTO pagination);

        Task<ServiceResult> DeleteUserFeedbackAsync(long userFeedbackId);
    }
}
