using BidMasterOnline.Domain.Models.Entities;
using IdentityServer.Models;

namespace IdentityServer.Services.Contracts
{
    public interface IUserManager
    {
        Task<User> GetByIdAsync(long id);
        Task<bool> ExistsWithUsernameAsync(string username);
        Task<bool> ExistsWithEmailAsync(string email);
        Task<User> CreateUserAsync(CreateUserModel userModel, string role);
    }
}
