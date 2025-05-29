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
    public class ComplaintsService : IComplaintsService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly ILogger<ComplaintsService> _logger;

        public ComplaintsService(IRepository repository, IUserAccessor userAccessor, ILogger<ComplaintsService> logger)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _logger = logger;
        }

        public async Task<ServiceResult<ComplaintDTO>> GetComplaintByIdAsync(long complaintId)
        {
            ServiceResult<ComplaintDTO> result = new();

            long userId = _userAccessor.UserId;

            Complaint? entity = await _repository
                .GetFirstOrDefaultAsync<Complaint>(e => e.Id == complaintId && e.AccusingUserId == userId);

            if (entity == null)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("Complaint not found.");
            }
            else
            {
                result.Data = entity.ToParticipantDTO();
            }

            return result;
        }

        public async Task<ServiceResult<PaginatedList<SummaryComplaintDTO>>> GetUserComplaintsAsync(
            PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<SummaryComplaintDTO>> result = new();

            long userId = _userAccessor.UserId;

            ISpecification<Complaint> specification = new SpecificationBuilder<Complaint>()
                .With(e => e.AccusingUserId == userId)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .OrderBy(e => e.CreatedAt)
                .Build();

            ListModel<Complaint> entitiesList = await _repository.GetFilteredAndPaginated(specification);

            result.Data = entitiesList.ToPaginatedList(e => e.ToParticipantSummaryDTO());

            return result;
        }

        public async Task<ServiceResult> PostComplaintAsync(PostComplaintDTO complaintDTO)
        {
            ServiceResult result = new();

            try
            {
                Complaint complaint = complaintDTO.ToDomain();
                complaint.AccusingUserId = _userAccessor.UserId;
                complaint.Status = BidMasterOnline.Domain.Enums.ComplaintStatus.Pending;

                await _repository.AddAsync(complaint);
                await _repository.SaveChangesAsync();

                result.Message = "Your complaint has been successfully submitted.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during placing a complaint.");
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during placing a complaint.");
            }

            return result;
        }
    }
}
