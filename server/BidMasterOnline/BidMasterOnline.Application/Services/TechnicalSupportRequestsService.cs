using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.RepositoryContracts;
using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;

namespace BidMasterOnline.Application.Services
{
    public class TechnicalSupportRequestsService : ITechnicalSupportRequestsService
    {
        private readonly IRepository _repository;
        private readonly IAuthService _authService;

        public TechnicalSupportRequestsService(IRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<TechnicalSupportRequestDTO> GetTechnicalSupportRequestByIdAsync(Guid id)
        {
            var technicalSupportRequest = await _repository.GetByIdAsync<TechnicalSupportRequest>(id);

            if (technicalSupportRequest is null)
                throw new KeyNotFoundException("Technical support request with such id does not exist.");

            return new TechnicalSupportRequestDTO
            {
                Id = technicalSupportRequest.Id,
                UserId = technicalSupportRequest.UserId,
                Username = technicalSupportRequest.User.Username,
                DateAndTime = technicalSupportRequest.DateAndTime.ToString("yyyy-MM-dd HH:mm"),
                RequestText = technicalSupportRequest.RequestText,
                IsHandled = technicalSupportRequest.IsHandled
            };
        }

        public async Task<ListModel<TechnicalSupportRequestDTO>> GetTechnicalSupportRequestsListAsync(TechnicalSupportRequestSpecificationsDTO specifications)
        {
            if (specifications is null)
                throw new ArgumentNullException("Specifications are null.");

            var specification = new SpecificationBuilder<TechnicalSupportRequest>()
                .With(x => x.IsHandled == specifications.IsHandled)
                .OrderBy(x => x.DateAndTime, Enums.SortDirection.DESC)
                .WithPagination(specifications.PageSize, specifications.PageNumber)
                .Build();

            var technicalSupportRequests = await _repository.GetAsync<TechnicalSupportRequest>(specification);

            var totalCount = await _repository.CountAsync<TechnicalSupportRequest>(specification.Predicate);

            var totalPages = (long)Math.Ceiling((double)totalCount / specifications.PageSize);

            var list = new ListModel<TechnicalSupportRequestDTO>()
            {
                List = technicalSupportRequests
                    .Select(x => new TechnicalSupportRequestDTO
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Username = x.User.Username,
                        DateAndTime = x.DateAndTime.ToString("yyyy-MM-dd HH:mm"),
                        RequestText = x.RequestText,
                        IsHandled = x.IsHandled,
                    })
                    .ToList(),
                TotalPages = totalPages
            };

            return list;
        }

        public async Task HandleTechnicalSupportRequestAsync(Guid id)
        {
            var technicalSupportRequest = await _repository.GetByIdAsync<TechnicalSupportRequest>(id);

            if (technicalSupportRequest is null)
                throw new KeyNotFoundException("Technical support request with such id does not exist.");

            if (technicalSupportRequest.IsHandled)
                throw new InvalidOperationException("Technical support request is already handled.");

            technicalSupportRequest.IsHandled = true;

            await _repository.UpdateAsync(technicalSupportRequest);
        }

        public async Task SetTechnicalSupportRequestAsync(SetTechnicalSupportRequestDTO request)
        {
            if (request is null)
                throw new ArgumentNullException("Technical support request is null.");

            var user = await _authService.GetAuthenticatedUserEntityAsync();

            var technicalSupportRequest = new TechnicalSupportRequest
            {
                UserId = user.Id,
                RequestText = request.RequestText,
                DateAndTime = DateTime.Now,
                IsHandled = false
            };

            await _repository.AddAsync(technicalSupportRequest);
        }
    }
}
