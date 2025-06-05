using BidMasterOnline.Core.DTO;
using Users.Service.API.DTO.Moderator;

namespace Users.Service.API.ServiceContracts.Moderator
{
    public interface IUsersService
    {
        Task<ServiceResult<IEnumerable<ModeratorSummaryDTO>>> GetAllModerators();

        Task<ServiceResult<PaginatedList<UserProfileSummaryInfoDTO>>> GetUsersListAsync(UserSpecificationsDTO specifications);

        Task<ServiceResult<UserProfileInfoDTO>> GetUserProfileInfoAsync(long userId);

        Task<ServiceResult> BlockUserAsync(long userId, BlockUserDTO request);

        Task<ServiceResult> UnblockUserAsync(long userId, CancellationToken? token = null);
    }
}
