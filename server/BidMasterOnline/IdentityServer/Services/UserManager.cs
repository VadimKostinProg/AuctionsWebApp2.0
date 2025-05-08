using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Domain.Models.Entities;
using IdentityServer.Helpers;
using IdentityServer.Models;
using IdentityServer.Services.Contracts;

namespace IdentityServer.Services
{
    public class UserManager : IUserManager
    {
        private readonly IRepository _repository;

        public UserManager(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<User> CreateUserAsync(CreateUserModel userModel, string role)
        {
            string passwordSalt = CryptographyHelper.GenerateSalt(size: 128);
            string passwordHashed = CryptographyHelper.Hash(userModel.Password, passwordSalt);

            long roleId = await GetRoleIdByName(role);

            User user = new()
            {
                Username = userModel.Username,
                FullName = userModel.FullName,
                Email = userModel.Email,
                DateOfBirth = userModel.DateOfBirth,
                PasswordHashed = passwordHashed,
                PasswordSalt = passwordSalt,
                RoleId = roleId
            };

            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            return user;
        }

        public Task<bool> ExistsWithUsernameAsync(string username)
            => _repository.AnyAsync<User>(e => e.Username == username);

        public Task<bool> ExistsWithEmailAsync(string email)
            => _repository.AnyAsync<User>(e => e.Email == email);

        public Task<User> GetByIdAsync(long id)
            => _repository.GetByIdAsync<User>(id);

        private async Task<long> GetRoleIdByName(string role)
            => (await _repository.GetFirstOrDefaultAsync<Role>(e => e.Name == role))!.Id;
    }
}
