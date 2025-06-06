using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Enums;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Auctions.Service.API.Services.Participant
{
    public class AuctionsService : IAuctionsService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly ITransactionsService _transactionsService;
        private readonly ILogger<AuctionsService> _logger;

        public AuctionsService(IRepository repository,
            IUserAccessor userAccessor,
            ITransactionsService transactionsService,
            ILogger<AuctionsService> logger)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _transactionsService = transactionsService;
            _logger = logger;
        }

        public async Task<ServiceResult<PaginatedList<AuctionSummaryDTO>>> GetAuctionsListAsync(AuctionSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<AuctionSummaryDTO>> result = new();

            ISpecification<Auction> specification = GetSpecification(specifications);

            ListModel<Auction> auctionsList = await _repository.GetFilteredAndPaginated(specification,
                includeQuery: query => query.Include(e => e.Category)
                                            .Include(e => e.Auctioneer)
                                            .Include(e => e.Images)
                                            .Include(e => e.Bids)!);

            result.Data = auctionsList.ToPaginatedList(e => e.ToParticipantSummaryDTO());

            return result;
        }

        public async Task<ServiceResult<PaginatedList<AuctionSummaryDTO>>> GetUserAuctionsAsync(PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<AuctionSummaryDTO>> result = new();

            long userId = _userAccessor.UserId;

            ISpecification<Auction> specification = new SpecificationBuilder<Auction>()
                .With(x => x.AuctioneerId == userId)
                .OrderBy(x => x.StartTime, SortDirection.DESC)
                .WithPagination(pagination.PageSize, pagination.PageNumber)
                .Build();

            ListModel<Auction> auctionsList = await _repository.GetFilteredAndPaginated(specification,
                includeQuery: query => query.Include(e => e.Category)!);

            result.Data = auctionsList.ToPaginatedList(e => e.ToParticipantSummaryDTO());

            return result;
        }

        public async Task<ServiceResult<AuctionDTO>> GetAuctionByIdAsync(long id)
        {
            ServiceResult<AuctionDTO> result = new();

            Auction? auction = await _repository.GetFirstOrDefaultAsync<Auction>(x => x.Id == id && x.Status != AuctionStatus.Pending,
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

            result.Data = auction.ToParticipantDTO();

            return result;
        }

        public virtual async Task<ServiceResult> CancelAuctionAsync(CancelAuctionDTO request)
        {
            ServiceResult result = new();

            long userId = _userAccessor.UserId;

            Auction? auction = await _repository
                .GetFirstOrDefaultAsync<Auction>(x => x.Id == request.AuctionId && x.AuctioneerId == userId);

            if (auction == null)
            {
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.IsSuccessfull = false;
                result.Errors.Add("Auction not found.");

                return result;
            }

            auction.Status = BidMasterOnline.Domain.Enums.AuctionStatus.CancelledByAuctioneer;
            auction.CancellationReason = request.CancellationReason;
            auction.FinishTime = DateTime.UtcNow;
            _repository.Update(auction);

            await _repository.SaveChangesAsync();

            result.Message = "Auction has been cancelled successfully!";

            return result;
        }

        public async Task<bool> FinishAuctionAsync(long id, CancellationToken? token = null)
        {
            Auction auction = await _repository.GetByIdAsync<Auction>(id,
                includeQuery: query => query.Include(e => e.Bids)!);

            Bid? winningBid = auction.Bids!.OrderByDescending(b => b.CreatedAt)
                .FirstOrDefault();

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                auction.Status = AuctionStatus.Finished;

                if (winningBid != null)
                {
                    auction.WinnerId = winningBid.BidderId;
                    auction.FinishPrice = winningBid.Amount;

                    User auctionist = await _repository.GetByIdAsync<User>(auction.AuctioneerId);
                    User winner = await _repository.GetByIdAsync<User>(winningBid.BidderId);

                    auctionist.CompletedAuctions++;
                    winner.TotalWins++;

                    _repository.Update(auctionist);
                    _repository.Update(winner);

                    // TODO: send notification to auctionist
                    // TODO: send notification to winner
                }
                else
                {
                    // TODO: send notification (no winner)
                }

                _repository.Update(auction);
                await _repository.SaveChangesAsync();

                if (token != null) await transaction.CommitAsync(token.Value);
                else await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                if (token != null) await transaction.RollbackAsync(token.Value);
                else await transaction.RollbackAsync();

                _logger.LogError(ex, "An error occured while finishing auction.");

                return false;
            }
        }

        public async Task<bool> SwitchAuctionToActiveAsync(long id)
        {
            Auction auction = await _repository.GetByIdAsync<Auction>(id);

            try
            {
                auction.Status = AuctionStatus.Active;

                // TODO: send notification to auctionist

                _repository.Update(auction);
                await _repository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while finishing auction.");

                return false;
            }
        }

        private ISpecification<Auction> GetSpecification(AuctionSpecificationsDTO specifications)
        {
            var builder = new SpecificationBuilder<Auction>();

            builder.With(x => x.Status != AuctionStatus.Pending);

            if (specifications.CategoryId is not null)
                builder.With(x => x.AuctionCategoryId == specifications.CategoryId);

            if (specifications.TypeId is not null)
                builder.With(x => x.AuctionTypeId == specifications.TypeId);

            if (specifications.MinStartPrice is not null)
                builder.With(x => x.StartPrice >= specifications.MinStartPrice && x.StartPrice <= specifications.MaxStartPrice!.Value);

            if (specifications.MinCurrentPrice is not null)
                builder.With(x => x.CurrentPrice >= specifications.MinCurrentPrice && x.CurrentPrice <= specifications.MaxCurrentPrice!.Value);

            if (specifications.AuctionStatus is not null)
                builder.With(x => x.Status == specifications.AuctionStatus);

            if (!string.IsNullOrEmpty(specifications.SearchTerm))
                builder.With(x => x.LotTitle.Contains(specifications.SearchTerm) || x.LotDescription.Contains(specifications.SearchTerm));

            // TODO: implement sorting

            if (!string.IsNullOrEmpty(specifications.SortBy))
            {
                switch (specifications.SortBy)
                {
                    case "popularity":
                        builder.With(x => x.Status == AuctionStatus.Active);
                        builder.OrderBy(x => x.Bids!.Count(), SortDirection.DESC);
                        break;
                    case "finishTime":
                        builder.With(x => x.Status == AuctionStatus.Active);
                        builder.OrderBy(x => x.FinishTime, SortDirection.ASC);
                        break;
                }
            }

            builder.WithPagination(specifications.PageSize, specifications.PageNumber);

            return builder.Build();
        }
    }
}
