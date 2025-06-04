using BidMasterOnline.Core.DTO;
using Users.Service.API.DTO.Moderator;

namespace Users.Service.API.ServiceContracts.Moderator
{
    public interface IUserProfilesService
    {
        Task<ServiceResult<UserProfileInfoDTO>> GetUserProfileInfoAsync(long userId);

        Task<ServiceResult> BlockUserAsync(BlockUserDTO request);

        Task<ServiceResult> UnblockUserAsync(long userId, CancellationToken? token = null);
    }
}
