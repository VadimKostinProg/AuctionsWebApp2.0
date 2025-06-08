using BidMasterOnline.Core.DTO;
using Users.Service.API.DTO.Participant;

namespace Users.Service.API.ServiceContracts.Participant
{
    public interface IUserProfilesService
    {
        Task<ServiceResult<UserProfileInfoDTO>> GetUserProfileInfoAsync(long userId);

        Task<ServiceResult<ExtendedUserProfileInfoDTO>> GetOwnUserProfileInfoAsync();

        Task<ServiceResult> ResetPasswordAsync(ResetPasswordDTO request);

        Task<ServiceResult> DeleteProfileAsync();
    }
}
