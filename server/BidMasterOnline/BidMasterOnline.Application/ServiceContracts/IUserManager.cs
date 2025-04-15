using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Enums;

namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing users.
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Gets customers list with applyed specifications.
        /// </summary>
        /// <param name="specifications">Specifications of searching, sorting, pagination, filtering by status to apply.</param>
        /// <returns>Users list with applyed specifications.</returns>
        Task<ListModel<UserDTO>> GetCustomersListAsync(UserSpecificationsDTO specifications);

        /// <summary>
        /// Gets staff users list with applyed specifications.
        /// </summary>
        /// <param name="specifications">Specifications of searching, sorting and pagination.</param>
        /// <returns>Users list with applyed specifications.</returns>
        Task<ListModel<UserDTO>> GetStaffListAsync(SpecificationsDTO specifications);

        /// <summary>
        /// Gets users profile information by it`s identifier.
        /// </summary>
        /// <param name="id">Identifier of user to get profile information of.</param>
        /// <returns>Users profiles information.</returns>
        Task<ProfileDTO> GetUserProfileByIdAsync(Guid id);

        /// <summary>
        /// Create new user of specified role.
        /// </summary>
        /// <param name="request">Information of user to create.</param>
        /// <param name="role">Role of user to create.</param>
        Task CreateUserAsync(CreateUserDTO request, UserRole role);

        /// <summary>
        /// Confirms the email of the authenticated user.
        /// </summary>
        /// <param name="userId">Identifier of user to confirm email.</param>
        Task ConfirmEmailAsync(Guid userId);

        /// <summary>
        /// Changes the password of the specified user.
        /// </summary>
        /// <param name="request">Object with password information.</param>
        Task ChangePasswordAsync(ChangePasswordDTO request);

        /// <summary>
        /// Blocks the specified user for the specified amount of days.
        /// </summary>
        /// <param name="request">Information of user to block and reason .</param>
        Task BlockUserAsync(BlockUserDTO request);

        /// <summary>
        /// Unblocks the specified user.
        /// </summary>
        /// <param name="userId">Identiier of user to unblock.</param>
        Task UnblockUserAsync(Guid userId);

        /// <summary>
        /// Applyes soft deleting of the currently authenticated user.
        /// </summary>
        Task DeleteUserAsync();

        /// <summary>
        /// Applyes soft deleting of the specified user.
        /// </summary>
        /// <param name="userId">Identifier of user to delete.</param>
        Task DeleteUserAsync(Guid userId);
    }
}
