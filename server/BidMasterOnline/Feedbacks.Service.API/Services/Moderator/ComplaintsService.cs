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
using Feedbacks.Service.API.ServiceContracts;
using Feedbacks.Service.API.ServiceContracts.Moderator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Feedbacks.Service.API.Services.Moderator
{
    public class ComplaintsService : IComplaintsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<ComplaintsService> _logger;
        private readonly ITransactionsService _transactionsService;
        private readonly IUserAccessor _userAccessor;
        private readonly INotificationsService _notificationsService;

        public ComplaintsService(IRepository repository,
            ILogger<ComplaintsService> logger,
            ITransactionsService transactionsService,
            IUserAccessor userAccessor,
            INotificationsService notificationsService)
        {
            _repository = repository;
            _logger = logger;
            _transactionsService = transactionsService;
            _userAccessor = userAccessor;
            _notificationsService = notificationsService;
        }

        public async Task<ServiceResult> AssignComplaintAsync(AssignComplaintDTO requestDTO)
        {
            ServiceResult result = new();

            try
            {
                Complaint complaint = await _repository.GetByIdAsync<Complaint>(requestDTO.ComplaintId);

                if (complaint.Status != ComplaintStatus.Pending && 
                    complaint.ModeratorId != _userAccessor.UserId)
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    result.Errors.Add("Could not change complaint status or reassing it, " +
                        "as it is assigned to another moderator.");

                    return result;
                }

                complaint.Status = ComplaintStatus.Active;
                complaint.ModeratorId = requestDTO.ModeratorId;
                complaint.ModeratorConclusion = null;

                _repository.Update(complaint);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while assinging the complaint.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("Could not reassign completed complaints.");
            }

            return result;
        }

        public async Task<ServiceResult> CompleteComplaintAsync(CompleteComplaintDTO requestDTO)
        {
            ServiceResult result = new();

            Complaint complaint = await _repository.GetByIdAsync<Complaint>(requestDTO.ComplaintId);

            long userId = _userAccessor.UserId;

            if (complaint.Status != ComplaintStatus.Active)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add("Could not complete non active complaints.");

                return result;
            }
            else if (complaint.ModeratorId != userId)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add("Could not complete complaints of another moderator.");

                return result;
            }

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                complaint.Status = ComplaintStatus.Completed;
                complaint.ModeratorConclusion = requestDTO.ModeratorConclusion;

                _repository.Update(complaint);
                await _repository.SaveChangesAsync();

                await _notificationsService.SendMessageOfCompletingComplaintAsync(complaint);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Error while assinging the complaint.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("Could not reassign completed complaints.");
            }

            return result;
        }

        public async Task<ServiceResult<ComplaintDTO>> GetComplaintByIdAsync(long complaintId)
        {
            ServiceResult<ComplaintDTO> result = new(); 
            
            Complaint entity = await _repository.GetByIdAsync<Complaint>(complaintId,
                includeQuery: query => query.Include(e => e.AccusingUser)
                                            .Include(e => e.AccusedUser)
                                            .Include(e => e.AccusedAuction)
                                            .Include(e => e.AccusedComment)
                                                .ThenInclude(c => c.User)
                                            .Include(e => e.AccusedUserFeedback)
                                                .ThenInclude(f => f.FromUser)
                                            .Include(e => e.AccusedUserFeedback)
                                                .ThenInclude(f => f.ToUser)
                                            .Include(e => e.Moderator)!);

            result.Data = entity.ToModeratorDTO();

            return result;
        }

        public async Task<ServiceResult<PaginatedList<SummaryComplaintDTO>>> GetComplaintsAsync(ComplaintsSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<SummaryComplaintDTO>> result = new();

            SpecificationBuilder<Complaint> specificationBuilder = new();

            if (specifications.ModeratorId.HasValue)
                specificationBuilder.With(e => e.ModeratorId == specifications.ModeratorId);

            if (specifications.Type.HasValue)
                specificationBuilder.With(e => e.Type == specifications.Type);

            if (specifications.Status.HasValue)
                specificationBuilder.With(e => e.Status == specifications.Status);

            specificationBuilder.WithPagination(specifications.PageSize, specifications.PageNumber);

            specificationBuilder.OrderBy(e => e.CreatedBy, BidMasterOnline.Core.Enums.SortDirection.DESC);

            ListModel<Complaint> entitiesList = await _repository.GetFilteredAndPaginated(
                specificationBuilder.Build(), 
                includeQuery: query => query.Include(e => e.AccusingUser)
                                            .Include(e => e.Moderator)!);

            result.Data = entitiesList.ToPaginatedList(e => e.ToModeratorSummaryDTO());

            return result;
        }
    }
}
