using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Feedbacks.Service.API.DTO.Participant;
using Feedbacks.Service.API.Extensions;
using Feedbacks.Service.API.ServiceContracts;
using Feedbacks.Service.API.ServiceContracts.Participant;

namespace Feedbacks.Service.API.Services.Participant
{
    public class SupportTicketsService : ISupportTicketsService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly ILogger<SupportTicketsService> _logger;
        private readonly INotificationsService _notificationsSerivce;

        public SupportTicketsService(IRepository repository,
            IUserAccessor userAccessor,
            ILogger<SupportTicketsService> logger,
            INotificationsService notificationsSerivce)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _logger = logger;
            _notificationsSerivce = notificationsSerivce;
        }

        public async Task<ServiceResult<SupportTicketDTO>> GetSupportTicketByIdAsync(long ticketId)
        {
            ServiceResult<SupportTicketDTO> result = new();

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

        public async Task<ServiceResult<PaginatedList<SummarySupportTicketDTO>>> GetUserSupportTicketsAsync(PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<SummarySupportTicketDTO>> result = new();

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

        public async Task<ServiceResult> PostSupportTicketAsync(PostSupportTicketDTO ticketDTO)
        {
            ServiceResult result = new();

            try
            {
                SupportTicket ticket = ticketDTO.ToDomain();
                ticket.UserId = _userAccessor.UserId;
                ticket.Status = BidMasterOnline.Domain.Enums.SupportTicketStatus.Pending;

                await _repository.AddAsync(ticket);
                await _repository.SaveChangesAsync();

                await _notificationsSerivce.SendMessageOfProcessingSupportTicketAsync(ticket);

                result.Message = "Your support ticket has been successfully submitted.";
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
