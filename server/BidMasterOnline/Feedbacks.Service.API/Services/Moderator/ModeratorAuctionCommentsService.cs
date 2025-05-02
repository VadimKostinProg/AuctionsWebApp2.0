using BidMasterOnline.Core.DTO;
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
using Microsoft.EntityFrameworkCore.Storage;

namespace Feedbacks.Service.API.Services.Moderator
{
    public class ModeratorAuctionCommentsService : IModeratorAuctionCommentsService
    {
        private readonly IRepository _repository;
        private readonly ModerationClient _moderationClient;
        private readonly ILogger<ModeratorAuctionCommentsService> _logger;
        private readonly ITransactionsService _transactionsService;

        public ModeratorAuctionCommentsService(IRepository repository,
            ModerationClient moderationClient,
            ILogger<ModeratorAuctionCommentsService> logger,
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

                await _moderationClient.LogModerationAction(ModerationAction.DeletingComment, commentId);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "An error occured while deleting a comment.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured while deleting a comment.");
            }

            return result;
        }

        public async Task<ServiceResult<PaginatedList<ModeratorAuctionCommentDTO>>> GetAuctionCommentsAsync(
            long auctionId, PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<ModeratorAuctionCommentDTO>> result = new();

            ISpecification<AuctionComment> specifications = new SpecificationBuilder<AuctionComment>()
                .With(e => e.AuctionId == auctionId)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                .Build();

            ListModel<AuctionComment> entitiesList = await _repository.GetFilteredAndPaginated(specifications);

            result.Data = new PaginatedList<ModeratorAuctionCommentDTO>
            {
                Items = entitiesList.Items.Select(e => e.ToModeratorDTO()).ToList(),
                Pagination = new Pagination()
                {
                    TotalCount = entitiesList.TotalCount,
                    TotalPages = entitiesList.TotalPages,
                    PageSize = entitiesList.PageSize,
                    CurrentPage = entitiesList.CurrentPage
                }
            };

            return result;
        }
    }
}
