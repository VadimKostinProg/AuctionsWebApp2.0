using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.Helpers;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models.Entities;
using IdentityServer.Constants;
using IdentityServer.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Services
{
    public class PasswordValidationService : IPasswordValidationService
    {
        private readonly IRepository _repository;

        public PasswordValidationService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> ValidateAsync(string username, string password, string? clientId = null)
        {
            long? roleId = null;

            if (!string.IsNullOrEmpty(clientId))
            {
                roleId = clientId switch
                {
                    IdentityServerClients.ParticipantUI => await GetRoleIdByName(UserRoles.Participant),
                    IdentityServerClients.ModeratorUI => await GetRoleIdByName(UserRoles.Moderator),
                    _ => null
                };
            }

            User? user = await _repository.GetFirstOrDefaultAsync<User>(
                u => (u.Email == username || u.Username == username) &&
                     (!roleId.HasValue || u.RoleId == roleId) &&
                     u.Status != UserStatus.Deleted,
                includeQuery: query => query.Include(e => e.Role)!);

            if (user != null && ValidatePassword(user, password))
            {
                return user;
            }

            return null;
        }

        private async Task<long> GetRoleIdByName(string role)
            => (await _repository.GetFirstOrDefaultAsync<Role>(e => e.Name == role))!.Id;

        private bool ValidatePassword(User user, string password)
            => user.PasswordHashed == CryptographyHelper.Hash(password, user.PasswordSalt);
    }
}
