using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Models.Entities;
using IdentityServer.Models;

namespace IdentityServer.Services.Contracts
{
    public interface IUserManager
    {
        Task<PaginatedList<User>> GetStaffListAsync(StaffListSpecifications specifications);
        Task<User> GetByIdAsync(long id);
        Task<bool> ExistsWithUsernameAsync(string username);
        Task<bool> ExistsWithEmailAsync(string email);
        Task<User> CreateUserAsync(CreateUserModel userModel, string role);
        Task DeleteUserAsync(long id);
    }
}
