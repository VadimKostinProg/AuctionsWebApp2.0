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
using Feedbacks.Service.API.ServiceContracts.Moderator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Feedbacks.Service.API.Services.Moderator
{
    public class SupportTicketsService : ISupportTicketsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<SupportTicketsService> _logger;
        private readonly ITransactionsService _transactionsService;
        private readonly IUserAccessor _userAccessor;

        public SupportTicketsService(IRepository repository, 
            ILogger<SupportTicketsService> logger, 
            ITransactionsService transactionsService, 
            IUserAccessor userAccessor)
        {
            _repository = repository;
            _logger = logger;
            _transactionsService = transactionsService;
            _userAccessor = userAccessor;
        }

        public async Task<ServiceResult> AssignSupportTicketAsync(AssignSupportTicketDTO requestDTO)
        {
            ServiceResult result = new();

            try
            {
                SupportTicket supportTicket = await _repository.GetByIdAsync<SupportTicket>(
                    requestDTO.SupportTicketId);

                if (supportTicket.Status != SupportTicketStatus.Pending &&
                    supportTicket.ModeratorId != requestDTO.ModeratorId)
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    result.Errors.Add("Could not change support ticket status or reassing it, " +
                        "as it is assigned to another moderator.");

                    return result;
                }

                supportTicket.Status = SupportTicketStatus.Active;
                supportTicket.ModeratorId = requestDTO.ModeratorId;

                _repository.Update(supportTicket);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while assinging the support ticket.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("Could not reassign completed support tickets.");
            }

            return result;
        }

        public async Task<ServiceResult> CompleteSupportTicketAsync(CompleteSupportTicketDTO requestDTO)
        {
            ServiceResult result = new();

            SupportTicket supportTicket = await _repository.GetByIdAsync<SupportTicket>(
                requestDTO.SupportTicketId);

            long userId = _userAccessor.UserId;

            if (supportTicket.Status != SupportTicketStatus.Active)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add("Could not complete non active support tickets.");

                return result;
            }
            else if (supportTicket.ModeratorId != userId)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add("Could not complete support tickets of another moderator.");

                return result;
            }

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                supportTicket.Status = SupportTicketStatus.Completed;
                supportTicket.ModeratorComment = requestDTO.ModeratorComment;

                _repository.Update(supportTicket);
                await _repository.SaveChangesAsync();

                //TODO: notify user

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "Error while assinging the supportTicket.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("Could not reassign completed supportTickets.");
            }

            return result;
        }

        public async Task<ServiceResult<SupportTicketDTO>> GetSupportTicketByIdAsync(long supportTicketId)
        {
            ServiceResult<SupportTicketDTO> result = new();

            SupportTicket entity = await _repository.GetByIdAsync<SupportTicket>(supportTicketId,
                includeQuery: query => query.Include(e => e.User)
                                            .Include(e => e.Moderator)!);

            result.Data = entity.ToModeratorDTO();

            return result;
        }

        public async Task<ServiceResult<PaginatedList<SummarySupportTicketDTO>>> GetSupportTicketsAsync(
            SupportTicketsSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<SummarySupportTicketDTO>> result = new();

            SpecificationBuilder<SupportTicket> specificationBuilder = new();

            if (specifications.ModeratorId.HasValue)
                specificationBuilder.With(e => e.ModeratorId == specifications.ModeratorId);

            if (specifications.Status.HasValue)
                specificationBuilder.With(e => e.Status == specifications.Status);

            specificationBuilder.WithPagination(specifications.PageSize, specifications.PageNumber);

            ListModel<SupportTicket> entitiesList = await _repository
                .GetFilteredAndPaginated(specificationBuilder.Build(),
                    includeQuery: query => query.Include(e => e.User)
                                                .Include(e => e.Moderator)!);

            result.Data = entitiesList.ToPaginatedList(e => e.ToModeratorSummaryDTO());

            return result;
        }
    }
}
