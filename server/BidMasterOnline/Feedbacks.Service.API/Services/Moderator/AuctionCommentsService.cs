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
    public class AuctionCommentsService : IAuctionCommentsService
    {
        private readonly IRepository _repository;
        private readonly ModerationClient _moderationClient;
        private readonly ILogger<AuctionCommentsService> _logger;
        private readonly ITransactionsService _transactionsService;

        public AuctionCommentsService(IRepository repository,
            ModerationClient moderationClient,
            ILogger<AuctionCommentsService> logger,
            ITransactionsService transactionsService)
        {
            _repository = repository;
            _moderationClient = moderationClient;
            _logger = logger;
            _transactionsService = transactionsService;
        }

        public async Task<ServiceResult> DeleteCommentAsync(long commentId)
        {
            ServiceResult result = new();

            AuctionComment? entity = await _repository
                .GetFirstOrDefaultAsync<AuctionComment>(e => e.Id == commentId && !e.Deleted);

            if (entity == null)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("Auction comment not found.");

                return result;
            }

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                entity.Deleted = true;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                await RecalculateAuctionAverageScoreAsync(entity.AuctionId);

                await transaction.CommitAsync();

                result.Message = "Auction comment has been deleted successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "An error occured while deleting a comment.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured while deleting a comment.");
            }

            await _moderationClient.LogModerationAction(ModerationAction.DeletingAuctionComment, commentId);

            return result;
        }

        public async Task<ServiceResult<PaginatedList<AuctionCommentDTO>>> GetAuctionCommentsAsync(
            long auctionId, PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<AuctionCommentDTO>> result = new();

            ISpecification<AuctionComment> specifications = new SpecificationBuilder<AuctionComment>()
                .With(e => e.AuctionId == auctionId && !e.Deleted)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                .Build();

            ListModel<AuctionComment> entitiesList = await _repository.GetFilteredAndPaginated(specifications,
                includeQuery: query => query.Include(e => e.User)!);

            result.Data = entitiesList.ToPaginatedList(e => e.ToModeratorDTO());

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
