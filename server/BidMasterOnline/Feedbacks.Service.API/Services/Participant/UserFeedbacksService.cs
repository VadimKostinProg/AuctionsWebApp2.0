using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Feedbacks.Service.API.DTO.Participant;
using Feedbacks.Service.API.Extensions;
using Feedbacks.Service.API.ServiceContracts.Participant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Feedbacks.Service.API.Services.Participant
{
    public class UserFeedbacksService : IUserFeedbacksService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly ILogger<UserFeedbacksService> _logger;
        private readonly ITransactionsService _transactionsService;
        private readonly IUserStatusValidationService _userStatusValidationService;

        public UserFeedbacksService(IRepository repository,
            IUserAccessor userAccessor,
            ILogger<UserFeedbacksService> logger,
            ITransactionsService transactionsService,
            IUserStatusValidationService userStatusValidationService)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _logger = logger;
            _transactionsService = transactionsService;
            _userStatusValidationService = userStatusValidationService;
        }

        public async Task<ServiceResult> DeleteUserFeedbackAsync(long userFeedbackId)
        {
            ServiceResult result = new();

            long userId = _userAccessor.UserId;

            UserFeedback? userFeedback = await _repository.GetFirstOrDefaultAsync<UserFeedback>(
                e => e.Id == userFeedbackId && e.FromUserId == userId);

            if (userFeedback == null)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("User feedback not found.");

                return result;
            }

            if (userFeedback.FromUserId != userId)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                result.Errors.Add("You could not delete feedbacks of other users.");

                return result;
            }

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                userFeedback.Deleted = true;

                _repository.Update(userFeedback);
                await _repository.SaveChangesAsync();

                await RecalculateUserAverageScore(userFeedback.ToUserId);

                await transaction.CommitAsync();

                result.Message = "User feedback has been successfully deleted.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "An error occured during deleting user feedback.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("An error occured during deleting user feedback.");
            }

            return result;
        }

        public async Task<ServiceResult<PaginatedList<UserFeedbackDTO>>> GetUserFeedbacksAsync(
            long userId, PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<UserFeedbackDTO>> result = new();

            ISpecification<UserFeedback> specification = new SpecificationBuilder<UserFeedback>()
                .With(e => e.ToUserId == userId && !e.Deleted)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                .Build();

            ListModel<UserFeedback> entitiesList = await _repository.GetFilteredAndPaginated(specification,
                includeQuery: query => query.Include(e => e.FromUser)!);

            result.Data = entitiesList.ToPaginatedList(e => e.ToParticipantDTO());

            return result;
        }

        public async Task<ServiceResult> PostUserFeedbackAsync(PostUserFeedbackDTO userFeedbackDTO)
        {
            ServiceResult result = new();

            if (!await _userStatusValidationService.IsActiveAsync())
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.Forbidden;
                result.Errors.Add("You have no rights to post user feedbacks for now.");

                return result;
            }

            long userId = _userAccessor.UserId;

            if (await _repository.AnyAsync<UserFeedback>(e =>
                    e.FromUserId == userId && e.ToUserId == userFeedbackDTO.ToUserId && !e.Deleted))
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add("You have already submitted feedback to this user.");

                return result;
            }

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                UserFeedback userFeedback = userFeedbackDTO.ToDomain();
                userFeedback.FromUserId = userId;

                await _repository.AddAsync(userFeedback);
                await _repository.SaveChangesAsync();

                await RecalculateUserAverageScore(userFeedbackDTO.ToUserId);

                await transaction.CommitAsync();

                result.Message = "User feedback has been successfully posted.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "An error occured during placing a user feedback.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during placing a user feedback.");
            }

            return result;
        }

        private async Task RecalculateUserAverageScore(long userId)
        {
            IEnumerable<int> scores = _repository.GetFiltered<UserFeedback>(e => e.ToUserId == userId && !e.Deleted)
                .Select(e => e.Score);

            User user = await _repository.GetByIdAsync<User>(userId);
            user.AverageScore = scores.Any() 
                ? Math.Round(scores.Average(), 1)
                : null;

            _repository.Update(user);
            await _repository.SaveChangesAsync();
        }
    }
}
