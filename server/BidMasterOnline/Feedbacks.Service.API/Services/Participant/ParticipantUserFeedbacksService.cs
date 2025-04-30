using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Feedbacks.Service.API.DTO.Participant;
using Feedbacks.Service.API.Extensions;
using Feedbacks.Service.API.ServiceContracts.Participant;

namespace Feedbacks.Service.API.Services.Participant
{
    public class ParticipantUserFeedbacksService : IParticipantUserFeedbacksService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly ILogger<ParticipantUserFeedbacksService> _logger;

        public ParticipantUserFeedbacksService(IRepository repository, 
            IUserAccessor userAccessor, 
            ILogger<ParticipantUserFeedbacksService> logger)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _logger = logger;
        }

        public async Task<ServiceResult> DeleteUserFeedbackAsync(long userFeedbackId)
        {
            ServiceResult result = new();

            long userId = _userAccessor.UserId;

            UserFeedback? userFeedback = await _repository.GetFirstOrDefaultAsync<UserFeedback>(
                e => e.Id == userFeedbackId && e.FromUserId == userId);

            if (userFeedback != null)
            {
                await _repository.AddAsync(userFeedback);
                await _repository.SaveChangesAsync();
            }
            else
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("User feedback not found.");
            }

            return result;
        }

        public async Task<ServiceResult<PaginatedList<ParticipantUserFeedbackDTO>>> GetUserFeedbacksAsync(
            long userId, PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<ParticipantUserFeedbackDTO>> result = new();

            ISpecification<UserFeedback> specification = new SpecificationBuilder<UserFeedback>()
                .With(e => e.ToUserId == userId)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .OrderBy(e => e.CreatedAt)
                .Build();

            ListModel<UserFeedback> entitiesList = await _repository.GetFilteredAndPaginated(specification);

            result.Data = new PaginatedList<ParticipantUserFeedbackDTO>
            {
                Items = entitiesList.Items.Select(e => e.ToParticipantDTO()).ToList(),
                Pagination = new()
                {
                    TotalCount = entitiesList.TotalCount,
                    TotalPages = entitiesList.TotalPages,
                    CurrentPage = entitiesList.CurrentPage,
                    PageSize = entitiesList.PageSize
                }
            };

            return result;
        }

        public async Task<ServiceResult> PostUserFeedbackAsync(ParticipantPostUserFeedbackDTO userFeedbackDTO)
        {
            ServiceResult result = new();

            try
            {
                UserFeedback userFeedback = userFeedbackDTO.ToDomain();

                await _repository.AddAsync(userFeedback);
                await _repository.SaveChangesAsync();

                // TODO: recalculate user average score
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during placing a user feedback.");
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during placing a user feedback.");
            }

            return result;
        }
    }
}
