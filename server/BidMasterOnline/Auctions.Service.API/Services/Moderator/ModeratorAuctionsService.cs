using Auctions.Service.API.DTO;
using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.GrpcServices.Client;
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
using Microsoft.EntityFrameworkCore.Storage;
using System.Net;

namespace Auctions.Service.API.Services.Moderator
{
    public class ModeratorAuctionsService : IModeratorAuctionsService
    {
        private readonly IRepository _repository;
        private readonly ITransactionsService _transactionService;
        private readonly ILogger<ModeratorAuctionsService> _logger;
        private readonly ModerationClient _moderationClient;
        private readonly BidsClient _bidsClient;

        public ModeratorAuctionsService(IRepository repository,
            ITransactionsService transactionService,
            ILogger<ModeratorAuctionsService> logger,
            ModerationClient moderationClient,
            BidsClient bidsClient)
        {
            _repository = repository;
            _transactionService = transactionService;
            _logger = logger;
            _moderationClient = moderationClient;
            _bidsClient = bidsClient;
        }

        public async Task<ServiceResult> CancelAuctionAsync(CancelAuctionDTO requestDTO)
        {
            ServiceResult result = new();

            IDbContextTransaction transaction = _transactionService.BeginTransaction();

            try
            {
                Auction entity = await _repository.GetByIdAsync<Auction>(requestDTO.AuctionId);

                if (entity.Status != AuctionStatus.Pending && entity.Status != AuctionStatus.Active)
                {
                    await transaction.RollbackAsync();

                    result.IsSuccessfull = false;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Errors.Add("Auction has been already cancelled or finished.");
                    return result;
                }

                entity.Status = AuctionStatus.CancelledByModerator;
                entity.FinishTime = DateTime.UtcNow;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                await _moderationClient.LogModerationAction(ModerationAction.CancelingAuction, entity.Id);

                // TODO: notify auctionist and auctioners

                await transaction.CommitAsync();

                result.Message = "Auction has been successfully cancelled!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while canceling auction.");

                result.IsSuccessfull = false;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occurred while canceling auction.");
            }

            return result;
        }

        public async Task<ServiceResult> RecoverAuctionAsync(RecoverAuctionDTO requestDTO)
        {
            ServiceResult result = new();

            IDbContextTransaction transaction = _transactionService.BeginTransaction();

            try
            {
                Auction entity = await _repository.GetByIdAsync<Auction>(requestDTO.AuctionId);

                if (entity.Status != AuctionStatus.CancelledByAuctionist && 
                    entity.Status != AuctionStatus.CancelledByModerator)
                {
                    await transaction.RollbackAsync();

                    result.IsSuccessfull = false;
                    result.StatusCode = HttpStatusCode.BadRequest;
                    result.Errors.Add("Auction is not cancelled.");
                    return result;
                }

                entity.Status = AuctionStatus.Active;
                entity.StartTime = DateTime.UtcNow;
                entity.FinishTime = entity.StartTime.AddTicks(entity.AuctionTimeInTicks);

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                await _moderationClient.LogModerationAction(ModerationAction.RecoveringAuction, entity.Id);

                await _bidsClient.ClearAllBidsForAuctionAsync(entity.Id);

                // TODO: notify auctionist and auctioners

                await transaction.CommitAsync();

                result.Message = "Auction has been successfully recovered.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while recovering auction.");

                result.IsSuccessfull = false;
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occurred while recovering auction.");
            }

            return result;
        }

        public async Task<ServiceResult<AuctionDTO>> GetAuctionByIdAsync(long id)
        {
            ServiceResult<AuctionDTO> result = new();

            Auction? auction = await _repository.GetFirstOrDefaultAsync<Auction>(x => x.Id == id,
                includeQuery: query => query.Include(e => e.Category)
                                            .Include(e => e.Type)
                                            .Include(e => e.FinishMethod)
                                            .Include(e => e.Auctionist)
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
                includeQuery: query => query.Include(e => e.Auctionist)
                                            .Include(e => e.Images)!);

            result.Data = auctionsList.ToPaginatedList(e => e.ToModeratorSummaryDTO());

            return result;
        }

        private ISpecification<Auction> GetSpecification(AuctionSpecificationsDTO specifications)
        {
            var builder = new SpecificationBuilder<Auction>();

            if (specifications.CategoryId is not null)
                builder.With(x => x.AuctionCategoryId == specifications.CategoryId);

            if (specifications.AuctionistId is not null)
                builder.With(x => x.AuctionistId == specifications.AuctionistId);

            if (specifications.MinStartPrice is not null)
                builder.With(x => x.StartPrice >= specifications.MinStartPrice && x.StartPrice <= specifications.MaxStartPrice!.Value);

            if (specifications.MinCurrentBid is not null)
                builder.With(x => x.CurrentPrice >= specifications.MinCurrentBid && x.CurrentPrice <= specifications.MaxCurrentBid!.Value);

            if (specifications.Status is not null)
                builder.With(x => x.Status == specifications.Status);

            if (specifications.WinnerId is not null)
                builder.With(x => x.WinnerId == specifications.WinnerId);

            if (!string.IsNullOrEmpty(specifications.SearchTerm))
                builder.With(x => x.LotTitle.Contains(specifications.SearchTerm) || x.LotDescription.Contains(specifications.SearchTerm));

            // TODO: implement sorting

            //if (!string.IsNullOrEmpty(specifications.SortField))
            //{
            //    switch (specifications.SortField)
            //    {
            //        case "popularity":
            //            builder.OrderBy(x => x.Bids.Count(), specifications.SortDirection ?? Enums.SortDirection.DESC);
            //            break;
            //        case "dateAndTime":
            //            builder.OrderBy(x => x.FinishDateTime, specifications.SortDirection ?? Enums.SortDirection.ASC);
            //            break;
            //    }
            //}

            builder.WithPagination(specifications.PageSize, specifications.PageNumber);

            return builder.Build();
        }
    }
}
