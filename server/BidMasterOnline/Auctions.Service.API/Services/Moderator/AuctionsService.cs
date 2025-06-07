using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.GrpcServices.Client;
using Auctions.Service.API.ServiceContracts;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Auctions.Service.API.Services.Moderator
{
    public class AuctionsService : IAuctionsService
    {
        private readonly IRepository _repository;
        private readonly ITransactionsService _transactionService;
        private readonly ILogger<AuctionsService> _logger;
        private readonly ModerationGrpcClient _moderationClient;
        private readonly BidsGrpcClient _bidsClient;
        private readonly INotificationsService _notificationsService;

        public AuctionsService(IRepository repository,
            ITransactionsService transactionService,
            ILogger<AuctionsService> logger,
            ModerationGrpcClient moderationClient,
            BidsGrpcClient bidsClient,
            INotificationsService notificationsService)
        {
            _repository = repository;
            _transactionService = transactionService;
            _logger = logger;
            _moderationClient = moderationClient;
            _bidsClient = bidsClient;
            _notificationsService = notificationsService;
        }

        public async Task<ServiceResult> CancelAuctionAsync(CancelAuctionDTO requestDTO)
        {
            ServiceResult result = new();

            try
            {
                Auction entity = await _repository.GetByIdAsync<Auction>(requestDTO.AuctionId);

                if (entity.Status != AuctionStatus.Pending && entity.Status != AuctionStatus.Active)
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Errors.Add("Auction has been already cancelled or finished.");
                    return result;
                }

                entity.Status = AuctionStatus.CancelledByModerator;
                entity.FinishTime = DateTime.UtcNow;
                entity.CancellationReason = requestDTO.Reason;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                await _notificationsService.SendMessageOfCancelingAuctionToAuctioneer(entity);

                result.Message = "Auction has been successfully cancelled!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while canceling auction.");

                result.IsSuccessfull = false;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occurred while canceling auction.");
            }

            await _moderationClient.LogModerationAction(ModerationAction.CancelingAuction, requestDTO.AuctionId);

            return result;
        }

        public async Task<ServiceResult> RecoverAuctionAsync(RecoverAuctionDTO requestDTO)
        {
            ServiceResult result = new();

            try
            {
                Auction entity = await _repository.GetByIdAsync<Auction>(requestDTO.AuctionId,
                    includeQuery: query => query.Include(e => e.Auctioneer!));

                if (entity.Status != AuctionStatus.CancelledByAuctioneer && 
                    entity.Status != AuctionStatus.CancelledByModerator)
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Errors.Add("Auction is not cancelled.");
                    return result;
                }

                if (entity.Auctioneer!.Status != UserStatus.Active)
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Errors.Add("Could not recover auction of non-active user.");
                    return result;
                }

                entity.Status = AuctionStatus.Active;
                entity.StartTime = DateTime.UtcNow;
                entity.FinishTime = entity.StartTime.AddTicks(entity.AuctionTimeInTicks);
                entity.CurrentPrice = entity.StartPrice;
                entity.CancellationReason = null;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                await _notificationsService.SendMessageOfRecoveringAuctionToAuctioneer(entity);

                result.Message = "Auction has been successfully recovered.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while recovering auction.");

                result.IsSuccessfull = false;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occurred while recovering auction.");
            }

            await _moderationClient.LogModerationAction(ModerationAction.RecoveringAuction, requestDTO.AuctionId);

            await _bidsClient.ClearAllBidsForAuctionAsync(requestDTO.AuctionId);

            return result;
        }

        public async Task<ServiceResult<AuctionDTO>> GetAuctionByIdAsync(long id)
        {
            ServiceResult<AuctionDTO> result = new();

            Auction? auction = await _repository.GetFirstOrDefaultAsync<Auction>(x => x.Id == id,
                includeQuery: query => query.Include(e => e.Category)
                                            .Include(e => e.Type)
                                            .Include(e => e.FinishMethod)
                                            .Include(e => e.Auctioneer)
                                            .Include(e => e.Winner)
                                            .Include(e => e.Images)!);

            if (auction == null)
            {
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.IsSuccessfull = false;
                result.Errors.Add("Auction not found.");

                return result;
            }

            result.Data = auction.ToModeratorDTO();

            return result;
        }

        public async Task<ServiceResult<PaginatedList<AuctionSummaryDTO>>> GetAuctionsListAsync(
            AuctionSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<AuctionSummaryDTO>> result = new();

            ISpecification<Auction> specification = GetSpecification(specifications);

            ListModel<Auction> auctionsList = await _repository.GetFilteredAndPaginated(specification,
                includeQuery: query => query.Include(e => e.Category)
                                            .Include(e => e.Type)!);

            result.Data = auctionsList.ToPaginatedList(e => e.ToModeratorSummaryDTO());

            return result;
        }

        public async Task<ServiceResult<PaginatedList<AuctionSummaryDTO>>> GetUserAuctionsAsync(long userId, 
            PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<AuctionSummaryDTO>> result = new();

            ISpecification<Auction> specification = new SpecificationBuilder<Auction>()
                .With(x => x.AuctioneerId == userId)
                .OrderBy(x => x.StartTime)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .Build();

            ListModel<Auction> auctionsList = await _repository.GetFilteredAndPaginated(specification,
                includeQuery: query => query.Include(e => e.Category)
                                            .Include(e => e.Type)!);

            result.Data = auctionsList.ToPaginatedList(e => e.ToModeratorSummaryDTO());

            return result;
        }

        public async Task<bool> CancelAllUserAuctionsAfterBlockingAsync(long userId)
        {
            try
            {
                List<Auction> userAuctions = await _repository
                    .GetFiltered<Auction>(e => e.AuctioneerId == userId && (e.Status == AuctionStatus.Pending || e.Status == AuctionStatus.Active))
                    .ToListAsync();

                userAuctions.ForEach(auction =>
                {
                    auction.Status = AuctionStatus.CancelledByModerator;
                    auction.FinishTime = DateTime.UtcNow;
                    auction.CancellationReason = "User's account has been blocked.";
                });

                await _repository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured during cancelling all auctions for user #{userId}.");

                return false;
            }
        }

        private ISpecification<Auction> GetSpecification(AuctionSpecificationsDTO specifications)
        {
            var builder = new SpecificationBuilder<Auction>();

            if (specifications.AuctionId is not null)
                builder.With(x => x.Id == specifications.AuctionId);

            if (specifications.CategoryId is not null)
                builder.With(x => x.AuctionCategoryId == specifications.CategoryId);

            if (specifications.TypeId is not null)
                builder.With(x => x.AuctionTypeId == specifications.TypeId);

            if (specifications.Status is not null)
                builder.With(x => x.Status == specifications.Status);

            if (!string.IsNullOrEmpty(specifications.SearchTerm))
                builder.With(x => x.LotTitle.Contains(specifications.SearchTerm) || x.LotDescription.Contains(specifications.SearchTerm));

            if (specifications.StartTime is not null)
            {
                DateTime startRange = specifications.StartTime.Value.Date;
                DateTime endRange = startRange.AddDays(1);

                builder.With(x => x.StartTime >= startRange && x.StartTime <= endRange);
            }

            if (specifications.FinishTime is not null)
            {
                DateTime startRange = specifications.FinishTime.Value.Date;
                DateTime endRange = startRange.AddDays(1);

                builder.With(x => x.FinishTime >= startRange && x.StartTime <= endRange);
            }

            if (!string.IsNullOrEmpty(specifications.SortBy))
            {
                switch (specifications.SortBy)
                {
                    case "id":
                        builder.OrderBy(x => x.Id, specifications.SortDirection);
                        break;
                    case "lotTitle":
                        builder.OrderBy(x => x.LotTitle, specifications.SortDirection);
                        break;
                    case "startTime":
                        builder.OrderBy(x => x.StartTime, specifications.SortDirection);
                        break;
                    case "finishTime":
                        builder.OrderBy(x => x.FinishTime, specifications.SortDirection);
                        break;
                    case "startPrice":
                        builder.OrderBy(x => x.StartPrice, specifications.SortDirection);
                        break;
                    case "currentPrice":
                        builder.OrderBy(x => x.CurrentPrice, specifications.SortDirection);
                        break;
                }
            }

            builder.WithPagination(specifications.PageSize, specifications.PageNumber);

            return builder.Build();
        }
    }
}
