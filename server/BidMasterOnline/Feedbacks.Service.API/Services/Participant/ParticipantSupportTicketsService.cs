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

namespace Feedbacks.Service.API.Services.Participant
{
    public class ParticipantSupportTicketsService : IParticipantSupportTicketsService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly ILogger<ParticipantSupportTicketsService> _logger;

        public ParticipantSupportTicketsService(IRepository repository,
            IUserAccessor userAccessor,
            ILogger<ParticipantSupportTicketsService> logger)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _logger = logger;
        }

        public async Task<ServiceResult<ParticipantSupportTicketDTO>> GetSupportTicketByIdAsync(long ticketId)
        {
            ServiceResult<ParticipantSupportTicketDTO> result = new();

            long userId = _userAccessor.UserId;
            SupportTicket? entity = await _repository
                .GetFirstOrDefaultAsync<SupportTicket>(e => e.Id == ticketId && e.UserId == userId);

            if (entity == null)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("SupportTicket not found.");
            }
            else
            {
                result.Data = entity.ToParticipantDTO();
            }

            return result;
        }

        public async Task<ServiceResult<PaginatedList<ParticipantSummarySupportTicketDTO>>> GetUserSupportTicketsAsync(PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<ParticipantSummarySupportTicketDTO>> result = new();

            long userId = _userAccessor.UserId;

            ISpecification<SupportTicket> specification = new SpecificationBuilder<SupportTicket>()
                .With(e => e.UserId == userId)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .OrderBy(e => e.CreatedAt)
                .Build();

            ListModel<SupportTicket> entitiesList = await _repository.GetFilteredAndPaginated(specification);

            result.Data = entitiesList.ToPaginatedList(e => e.ToParticipantSummaryDTO());

            return result;
        }

        public async Task<ServiceResult> PostSupportTicketAsync(ParticipantPostSupportTicketDTO ticketDTO)
        {
            ServiceResult result = new();

            try
            {
                SupportTicket ticket = ticketDTO.ToDomain();
                ticket.Status = BidMasterOnline.Domain.Enums.SupportTicketStatus.Pending;

                await _repository.AddAsync(ticket);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during placing a support ticket.");
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during placing a support ticket.");
            }

            return result;
        }
    }
}
