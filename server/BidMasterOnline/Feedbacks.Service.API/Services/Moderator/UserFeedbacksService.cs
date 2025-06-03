using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Feedbacks.Service.API.DTO.Moderator;
using Feedbacks.Service.API.Extensions;
using Feedbacks.Service.API.GrpcServices.Client;
using Feedbacks.Service.API.ServiceContracts.Moderator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Feedbacks.Service.API.Services.Moderator
{
    public class UserFeedbacksService : IUserFeedbacksService
    {
        private readonly IRepository _repository;
        private readonly ModerationClient _moderationClient;
        private readonly ITransactionsService _transactionsService;
        private readonly ILogger<UserFeedbacksService> _logger;

        public UserFeedbacksService(IRepository repository,
            ModerationClient moderationClient,
            ITransactionsService transactionsService,
            ILogger<UserFeedbacksService> logger)
        {
            _repository = repository;
            _moderationClient = moderationClient;
            _transactionsService = transactionsService;
            _logger = logger;
        }

        public async Task<ServiceResult> DeleteUserFeedbackAsync(long userFeedbackId)
        {
            ServiceResult result = new();

            UserFeedback? entity = await _repository
                .GetFirstOrDefaultAsync<UserFeedback>(e => e.Id == userFeedbackId && !e.Deleted);

            if (entity == null)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("User feedback not found.");

                return result;
            }

            try
            {
                entity.Deleted = true;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while deleting a user feedback.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured while deleting a user feedback.");
            }

            await _moderationClient.LogModerationAction(ModerationAction.DeletingUserFeedback, userFeedbackId);

            return result;
        }

        public async Task<ServiceResult<PaginatedList<UserFeedbackDTO>>> GetUserFeedbacksAsync(
            long userId,
            PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<UserFeedbackDTO>> result = new();

            ISpecification<UserFeedback> specifications = new SpecificationBuilder<UserFeedback>()
                .With(e => e.ToUserId == userId)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                .Build();

            ListModel<UserFeedback> entitiesList = await _repository.GetFilteredAndPaginated(specifications,
                includeQuery: query => query.Include(e => e.FromUser)
                                            .Include(e => e.ToUser)!);

            result.Data = entitiesList.ToPaginatedList(e => e.ToModeratorDTO());

            return result;
        }
    }
}
