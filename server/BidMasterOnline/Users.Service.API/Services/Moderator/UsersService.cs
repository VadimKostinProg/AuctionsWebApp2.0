using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Users.Service.API.DTO.Moderator;
using Users.Service.API.Extensions;
using Users.Service.API.GrpcServices.Client;
using Users.Service.API.ServiceContracts.Moderator;

namespace Users.Service.API.Services.Moderator
{
    public class UsersService : IUsersService
    {
        private readonly IRepository _repository;
        private readonly UserAuctionsGrpcClient _userAuctionsClient;
        private readonly UserBidsGrpcClient _userBidsClient;
        private readonly ILogger<UsersService> _logger;

        public UsersService(IRepository repository,
            UserAuctionsGrpcClient userAuctionsClient,
            UserBidsGrpcClient userBidsClient,
            ILogger<UsersService> logger)
        {
            _repository = repository;
            _userAuctionsClient = userAuctionsClient;
            _userBidsClient = userBidsClient;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<ModeratorSummaryDTO>>> GetAllModerators()
        {
            ServiceResult<IEnumerable<ModeratorSummaryDTO>> result = new();

            List<User> entities = await _repository.GetFiltered<User>(e => e.Role.Name == UserRoles.Moderator && !e.Deleted)
                .OrderBy(e => e.FullName)
                .ThenBy(e => e.Username)
                .ToListAsync();

            result.Data = entities.Select(e => e.ToModeratorSummaryDTO());

            return result;
        }

        public async Task<ServiceResult<PaginatedList<UserProfileSummaryInfoDTO>>> GetUsersListAsync(
            UserSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<UserProfileSummaryInfoDTO>> result = new();

            ListModel<User> usersList = await _repository.GetFilteredAndPaginated(GetSpecifications(specifications),
                includeQuery: query => query.Include(e => e.Role)!);

            result.Data = usersList.ToPaginatedList(e => e.ToModeratorUserProfileSummaryDTO());

            return result;
        }

        public async Task<ServiceResult> BlockUserAsync(long userId, BlockUserDTO request)
        {
            ServiceResult result = new();

            try
            {
                User? user = await _repository.GetFirstOrDefaultAsync<User>(e => e.Id == userId && e.Role!.Name == UserRoles.Participant,
                    includeQuery: query => query.Include(e => e.Role)!);

                if (user == null)
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.NotFound;
                    result.Errors.Add("User not found.");

                    return result;
                }

                if (user.Status != BidMasterOnline.Domain.Enums.UserStatus.Active)
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    result.Errors.Add("Could not block non active user.");

                    return result;
                }

                user.Status = BidMasterOnline.Domain.Enums.UserStatus.Blocked;
                user.BlockingReason = request.BlockingReason;

                if (request.BlockingPeriodInDays.HasValue && request.BlockingPeriodInDays > 0)
                {
                    user.UnblockDateTime = DateTime.Now.AddDays(request.BlockingPeriodInDays.Value);
                }

                _repository.Update(user);
                await _repository.SaveChangesAsync();

                await _userAuctionsClient.CancelUserAuctionsAsync(userId);
                await _userBidsClient.CancelUserWinningBidsAsync(userId);

                // TODO: Notify user

                result.Message = "User's account has been successfully blocked.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during blocking the user.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during blocking the user.");
            }

            return result;
        }

        public async Task<ServiceResult<UserProfileInfoDTO>> GetUserProfileInfoAsync(long userId)
        {
            ServiceResult<UserProfileInfoDTO> result = new();

            User? user = await _repository.GetFirstOrDefaultAsync<User>(e => e.Id == userId,
                    includeQuery: query => query.Include(e => e.Role)!);

            if (user == null)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("User not found.");

                return result;
            }

            result.Data = user.ToModeratorUserProfileDTO();

            return result;
        }

        public async Task<ServiceResult> UnblockUserAsync(long userId, CancellationToken? token = null)
        {
            ServiceResult result = new();

            try
            {
                User? user = await _repository.GetFirstOrDefaultAsync<User>(e => e.Id == userId);

                if (user == null)
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.NotFound;
                    result.Errors.Add("User not found.");

                    return result;
                }

                if (user.Status != BidMasterOnline.Domain.Enums.UserStatus.Blocked)
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    result.Errors.Add("Could not unblock non blocked user.");

                    return result;
                }

                user.Status = BidMasterOnline.Domain.Enums.UserStatus.Active;
                user.BlockingReason = null;
                user.UnblockDateTime = null;

                _repository.Update(user);
                await _repository.SaveChangesAsync();

                // TODO: Notify user

                result.Message = "User's account has been successfully unblocked.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during unblocking the user.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during unblocking the user.");
            }

            return result;
        }

        private ISpecification<User> GetSpecifications(UserSpecificationsDTO specifications)
        {
            SpecificationBuilder<User> specificationBuilder = new();

            specificationBuilder.With(e => e.Role!.Name == UserRoles.Participant);

            if (specifications.UserId.HasValue)
                specificationBuilder.With(e => e.Id == specifications.UserId);

            if (!string.IsNullOrEmpty(specifications.SearchTerm))
                specificationBuilder.With(e => e.Username.Contains(specifications.SearchTerm) ||
                                               e.FullName.Contains(specifications.SearchTerm) ||
                                               e.Email.Contains(specifications.SearchTerm));

            if (specifications.Status.HasValue) 
                specificationBuilder.With(e => e.Status == specifications.Status);

            if (!string.IsNullOrEmpty(specifications.SortBy))
            {
                switch(specifications.SortBy)
                {
                    case "id":
                        specificationBuilder.OrderBy(e => e.Id, specifications.SortDirection);
                        break;
                    case "username":
                        specificationBuilder.OrderBy(e => e.Username, specifications.SortDirection);
                        break;
                    case "fullName":
                        specificationBuilder.OrderBy(e => e.FullName, specifications.SortDirection);
                        break;
                    case "email":
                        specificationBuilder.OrderBy(e => e.Email, specifications.SortDirection);
                        break;
                    case "status":
                        specificationBuilder.OrderBy(e => e.Status, specifications.SortDirection);
                        break;
                }
            }

            specificationBuilder.WithPagination(specifications.PageSize, specifications.PageNumber);

            return specificationBuilder.Build();
        }
    }
}
