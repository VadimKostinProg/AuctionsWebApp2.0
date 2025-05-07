using BidMasterOnline.Domain.Models.Entities;

namespace IdentityServer.Services.Contracts
{
    public interface IPasswordValidationService
    {
        Task<User?> ValidateAsync(string username, string password);
    }
}
