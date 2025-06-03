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
    public class AuctionCommentsService : IAuctionCommentsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<AuctionCommentsService> _logger;
        private readonly ITransactionsService _transactionsService;
        private readonly IUserAccessor _userAccessor;
        private readonly IUserStatusValidationService _userStatusValidationService;

        public AuctionCommentsService(IRepository repository,
            ILogger<AuctionCommentsService> logger,
            ITransactionsService transactionsService,
            IUserAccessor userAccessor,
            IUserStatusValidationService userStatusValidationService)
        {
            _repository = repository;
            _logger = logger;
            _transactionsService = transactionsService;
            _userAccessor = userAccessor;
            _userStatusValidationService = userStatusValidationService;
        }

        public async Task<ServiceResult> DeleteCommentAsync(long commentId)
        {
            ServiceResult result = new();

            AuctionComment? comment = await _repository.GetFirstOrDefaultAsync<AuctionComment>(e => e.Id == commentId);

            if (comment == null)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("Comment not found.");

                return result;
            }

            if (comment.UserId != _userAccessor.UserId)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                result.Errors.Add("Comment not delete comment of another user.");

                return result;
            }

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                comment.Deleted = true;

                _repository.Update(comment);
                await _repository.SaveChangesAsync();

                await RecalculateAuctionAverageScoreAsync(comment.AuctionId);

                await transaction.CommitAsync();

                result.Message = "Auction comment has been successfully deleted.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "An error occured during deleting the comment.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during deleting the comment.");
            }

            return result;
        }

        public async Task<ServiceResult<PaginatedList<AuctionCommentDTO>>> GetAuctionCommentsAsync(long auctionId, PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<AuctionCommentDTO>> result = new();

            ISpecification<AuctionComment> specification = new SpecificationBuilder<AuctionComment>()
                .With(e => e.AuctionId == auctionId && !e.Deleted)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                .Build();

            ListModel<AuctionComment> entitiesList = await _repository.GetFilteredAndPaginated(specification,
                includeQuery: query => query.Include(e => e.User)!);

            result.Data = entitiesList.ToPaginatedList(e => e.ToParticipantDTO());

            return result;
        }

        public async Task<ServiceResult> PostAuctionCommentAsync(PostCommentDTO comment)
        {
            ServiceResult result = new();

            if (!await _userStatusValidationService.IsActiveAsync())
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.Forbidden;
                result.Errors.Add("You have no rights to post auction comments for now.");

                return result;
            }

            long userId = _userAccessor.UserId;

            if (await _repository.AnyAsync<AuctionComment>(e =>
                    e.UserId == userId && e.AuctionId == comment.AuctionId && !e.Deleted))
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add("You have already submitted cooment on this auction.");

                return result;
            }

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                AuctionComment entity = comment.ToDomain();

                entity.UserId = userId;

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                await RecalculateAuctionAverageScoreAsync(comment.AuctionId);

                await transaction.CommitAsync();

                result.Message = "Auction comment has been successfully posted.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "An error occured during posting the auction comment.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during posting the auction comment.");
            }

            return result;
        }

        private async Task RecalculateAuctionAverageScoreAsync(long auctionId)
        {
            IEnumerable<int> score = _repository.GetFiltered<AuctionComment>(x => x.AuctionId == auctionId && !x.Deleted)
                .Select(comment => comment.Score);

            Auction auction = await _repository.GetByIdAsync<Auction>(auctionId);
            auction.AverageScore = score.Any()
                ? Math.Round(score.Average(), 1)
                : null;

            _repository.Update(auction);
            await _repository.SaveChangesAsync();
        }
    }
}
