using BidMasterOnline.Application.DTO;
using BidMasterOnline.Domain.Entities;

namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for applying authentication for user.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Applyes user login.
        /// </summary>
        /// <param name="login">Users login information.</param>
        /// <returns>Auhtentication information of user with authorisation token.</returns>
        Task<AuthenticationDTO> LoginAsync(LoginDTO login);

        /// <summary>
        /// Gets authenticated user.
        /// </summary>
        /// <returns>Authenticated user.</returns>
        Task<UserDTO> GetAuthenticatedUserAsync();

        /// <summary>
        /// Gets the entity of the authenticated user.
        /// </summary>
        /// <returns>Entity of the authenticated user.</returns>
        Task<User> GetAuthenticatedUserEntityAsync();
    }
}
