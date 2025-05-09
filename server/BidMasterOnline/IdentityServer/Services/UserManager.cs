using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Enums;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
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

        public async Task<PaginatedList<User>> GetStaffListAsync(StaffListSpecifications specifications)
        {
            SpecificationBuilder<User> specificationBuilder = new();

            long moderatorRoleId = await GetRoleIdByName(UserRoles.Moderator);

            specificationBuilder.With(e => e.RoleId == moderatorRoleId);

            if (!string.IsNullOrEmpty(specifications.Search))
                specificationBuilder.With(e => e.Username.Contains(specifications.Search) ||
                                               e.FullName.Contains(specifications.Search) ||
                                               e.Email.Contains(specifications.Search));

            if (!string.IsNullOrEmpty(specifications.SortColumn) && !string.IsNullOrEmpty(specifications.SortDirection))
                specificationBuilder.OrderBy(
                        sortBy: specifications.SortColumn switch
                        {
                            "Username" => e => e.Username,
                            "FullName" => e => e.FullName,
                            "Email" => e => e.Email,
                            "DateOfBirth" => e => e.DateOfBirth,
                            _ => e => e.CreatedAt,
                        },
                        sortOrder: specifications.SortDirection switch
                        {
                            "asc" => SortDirection.ASC,
                            _ => SortDirection.DESC,
                        }
                    );

            specificationBuilder.WithPagination(specifications.PageSize, specifications.PageNumber);

            ListModel<User> usersList = await _repository.GetFilteredAndPaginated(specificationBuilder.Build());

            return new PaginatedList<User>
            {
                Items = usersList.Items,
                Pagination = new Pagination
                {
                    CurrentPage = usersList.CurrentPage,
                    PageSize = usersList.PageSize,
                    TotalCount = usersList.TotalCount,
                    TotalPages = usersList.TotalPages,
                }
            };
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

        public async Task DeleteUserAsync(long id)
        {
            User user = await _repository.GetByIdAsync<User>(id);

            user.Deleted = true;

            _repository.Update(user);
            await _repository.SaveChangesAsync();
        }

        private async Task<long> GetRoleIdByName(string role)
            => (await _repository.GetFirstOrDefaultAsync<Role>(e => e.Name == role))!.Id;
    }
}
