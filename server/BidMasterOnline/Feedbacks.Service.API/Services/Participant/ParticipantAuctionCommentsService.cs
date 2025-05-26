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
    public class ParticipantAuctionCommentsService : IParticipantAuctionCommentsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<ParticipantAuctionCommentsService> _logger;
        private readonly ITransactionsService _transactionsService;
        private readonly IUserAccessor _userAccessor;

        public ParticipantAuctionCommentsService(IRepository repository,
            ILogger<ParticipantAuctionCommentsService> logger,
            ITransactionsService transactionsService,
            IUserAccessor userAccessor)
        {
            _repository = repository;
            _logger = logger;
            _transactionsService = transactionsService;
            _userAccessor = userAccessor;
        }

        public async Task<ServiceResult> DeleteCommentAsync(long commentId)
        {
            ServiceResult result = new();

            try
            {
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

                comment.Deleted = true;

                _repository.Update(comment);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during deleting the comment.");
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during deleting the comment.");
            }

            return result;
        }

        public async Task<ServiceResult<PaginatedList<ParticipantAuctionCommentDTO>>> GetAuctionCommentsAsync(long auctionId, PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<ParticipantAuctionCommentDTO>> result = new();

            ISpecification<AuctionComment> specification = new SpecificationBuilder<AuctionComment>()
                .With(e => e.AuctionId == auctionId)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                .Build();

            ListModel<AuctionComment> entitiesList = await _repository.GetFilteredAndPaginated(specification,
                includeQuery: query => query.Include(e => e.User)!);

            result.Data = entitiesList.ToPaginatedList(e => e.ToParticipantDTO());

            return result;
        }

        public async Task<ServiceResult> PostAuctionCommentAsync(ParticipantPostCommentDTO comment)
        {
            ServiceResult result = new();

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                AuctionComment entity = comment.ToDomain();

                entity.UserId = _userAccessor.UserId;

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                await RecalculateAuctionAverageScoreAsync(comment.AuctionId);

                await transaction.CommitAsync();
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
            decimal avgScore = (decimal)_repository.GetFiltered<AuctionComment>(x => x.AuctionId == auctionId)
                .Select(comment => comment.Score)
                .Average();

            Auction auction = await _repository.GetByIdAsync<Auction>(auctionId);
            auction.AverageScore = (double)avgScore;
            _repository.Update(auction);
            await _repository.SaveChangesAsync();
        }
    }
}
