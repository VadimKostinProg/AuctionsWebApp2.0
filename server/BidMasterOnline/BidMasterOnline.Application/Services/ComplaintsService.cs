using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.RepositoryContracts;
using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;

namespace BidMasterOnline.Application.Services
{
    public class ComplaintsService : IComplaintsService
    {
        private readonly IRepository _repository;
        private readonly IAuthService _authService;

        public ComplaintsService(IRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<ComplaintDTO> GetComplaintByIdAsync(Guid id)
        {
            var complaint = await _repository.GetByIdAsync<Complaint>(id);

            if (complaint is null)
                throw new KeyNotFoundException("Complaint with such id does not exists.");

            return this.ConvertToComplaintDTO(complaint);
        }

        private ComplaintDTO ConvertToComplaintDTO(Complaint complaint)
        {
            var accusedComment = complaint.Comment;

            var complaintDTO = new ComplaintDTO
            {
                Id = complaint.Id,
                AccusedUserId = complaint.AccusedUserId,
                AccusedUsername = complaint.AccusedUser.Username,
                AccusingUserId = complaint.AccusingUserId,
                AccusingUsername = complaint.AccusingUser.Username,
                AuctionId = complaint.AuctionId,
                AuctionName = complaint.Auction.Name,
                CommentId = complaint.CommentId,
                Comment = accusedComment is not null ?
                    new CommentDTO
                    {
                        Id = accusedComment.Id,
                        UserId = accusedComment.UserId,
                        Username = accusedComment.User.Username,
                        AuctionId = accusedComment.Auction.Id,
                        AuctionName = accusedComment.Auction.Name,
                        DateAndTime = accusedComment.DateAndTime.ToString("yyyy-MM-dd HH:mm"),
                        CommentText = accusedComment.CommentText,
                        IsDeleted = accusedComment.IsDeleted,
                    }
                    : null,
                ComplaintType = Enum.Parse<Enums.ComplaintType>(complaint.ComplaintType.Name),
                ComplaintTypeDescription = complaint.ComplaintType.Description,
                DateAndTime = complaint.DateAndTime.ToString("yyyy-MM-dd HH:mm"),
                ComplaintText = complaint.ComplaintText,
            };

            return complaintDTO;
        }

        public async Task<ListModel<ComplaintDTO>> GetComplaintsListAsync(ComplaintSpecificationsDTO specificationsDTO)
        {
            var specification = new SpecificationBuilder<Complaint>()
                .With(x => x.ComplaintType.Name == specificationsDTO.Type.ToString())
                .With(x => !x.IsHandled)
                .OrderBy(x => x.DateAndTime, Enums.SortDirection.ASC)
                .WithPagination(specificationsDTO.PageSize, specificationsDTO.PageNumber)
                .Build();

            var complaints = await _repository.GetAsync<Complaint>(specification);

            var totalCount = await _repository.CountAsync<Complaint>(specification.Predicate);
            var totalPages = (long)Math.Ceiling((double)totalCount / specificationsDTO.PageSize);

            var list = new ListModel<ComplaintDTO>
            {
                List = complaints
                    .Select(x => this.ConvertToComplaintDTO(x))
                    .ToList(),
                TotalPages = totalPages
            };

            return list;
        }

        public async Task HandleComplaintAsync(Guid id)
        {
            var complaint = await _repository.GetByIdAsync<Complaint>(id);

            if (complaint is null)
                throw new KeyNotFoundException("Complaint with such id does not exist.");

            if (complaint.IsHandled)
                throw new InvalidOperationException("Complaint is already handled.");

            complaint.IsHandled = true;
            await _repository.UpdateAsync(complaint);
        }

        public async Task SetNewComplaintAsync(SetComplaintDTO request)
        {
            if (request is null)
                throw new ArgumentNullException("Complain is null.");

            if (string.IsNullOrEmpty(request.ComplaintText))
                throw new ArgumentNullException("Complaint text is blank.");

            var auction = await _repository.GetByIdAsync<Auction>(request.AuctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            var accusingUser = await _authService.GetAuthenticatedUserEntityAsync();

            if (request.ComplaintType == Enums.ComplaintType.ComplaintOnAuctionContent &&
                auction.AuctionistId == accusingUser.Id)
                throw new InvalidOperationException("You cannot set complaint for your auction.");

            if (!await _repository.AnyAsync<User>(x => x.Id == request.AccusedUserId))
                throw new KeyNotFoundException("User with such id does not exist.");

            // Validate request for comment
            if (request.ComplaintType == Enums.ComplaintType.ComplaintOnUserComment)
            {
                if (request.CommentId is null)
                    throw new ArgumentNullException("Comment is null.");

                var comment = await _repository.GetByIdAsync<AuctionComment>(request.CommentId.Value);

                if (comment is null)
                    throw new KeyNotFoundException("Comment with such id does not exist.");

                if (comment.UserId == accusingUser.Id)
                    throw new InvalidOperationException("You cannot set complaint on your comment.");
            }


            var complaintType = await _repository.FirstOrDefaultAsync<ComplaintType>(x =>
                x.Name == request.ComplaintType.ToString());

            var complaint = new Complaint
            {
                AccusingUserId = accusingUser.Id,
                AccusedUserId = request.AccusedUserId,
                AuctionId = request.AuctionId,
                CommentId = request.CommentId,
                DateAndTime = DateTime.Now,
                ComplaintText = request.ComplaintText,
                ComplaintTypeId = complaintType!.Id
            };

            await _repository.AddAsync(complaint);
        }
    }
}
