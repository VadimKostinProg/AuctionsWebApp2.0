using Auctions.Service.API.DTO;
using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
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
                                            .Include(e => e.Auctionist)
                                            .Include(e => e.Images)
                                            .Include(e => e.Bids)!);

            result.Data = auctionsList.ToPaginatedList(e => e.ToParticipantSummaryDTO());

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

            result.Data = auction.ToParticipantDTO();

            return result;
        }

        public virtual async Task<ServiceResult> CancelAuctionAsync(CancelAuctionDTO request)
        {
            ServiceResult result = new();

            long userId = _userAccessor.UserId;

            Auction? auction = await _repository
                .GetFirstOrDefaultAsync<Auction>(x => x.Id == request.AuctionId && x.AuctionistId == userId);

            if (auction == null)
            {
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.IsSuccessfull = false;
                result.Errors.Add("Auction not found.");

                return result;
            }

            auction.Status = BidMasterOnline.Domain.Enums.AuctionStatus.CancelledByAuctionist;
            auction.CancellationReason = request.CancellationReason;
            _repository.Update(auction);

            await _repository.SaveChangesAsync();

            result.Message = "Auction has been cancelled successfully!";

            return result;
        }

        public async Task<bool> FinishAuctionAsync(long id)
        {
            Auction auction = await _repository.GetByIdAsync<Auction>(id,
                includeQuery: query => query.Include(e => e.Bids)!);

            Bid? winningBid = auction.Bids!.OrderByDescending(b => b.CreatedAt)
                .FirstOrDefault();

            IDbContextTransaction transaction = _transactionsService.BeginTransaction();

            try
            {
                auction.Status = BidMasterOnline.Domain.Enums.AuctionStatus.Finished;

                if (winningBid != null)
                {
                    auction.WinnerId = winningBid.BidderId;
                    auction.FinishPrice = winningBid.Amount;

                    // TODO: send notification to auctionist
                    // TODO: send notification to winner
                }
                else
                {
                    // TODO: send notification (no winner) wrap to try catch
                }

                _repository.Update(auction);
                await _repository.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError(ex, "An error occured while finishing auction.");

                return false;
            }
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

            if (!string.IsNullOrEmpty(specifications.SortBy))
            {
                switch (specifications.SortBy)
                {
                    case "popularity":
                        builder.OrderBy(x => x.Bids!.Count(), specifications.SortDirection);
                        break;
                    case "finishTime":
                        builder.OrderBy(x => x.FinishTime, specifications.SortDirection);
                        break;
                }
            }

            builder.WithPagination(specifications.PageSize, specifications.PageNumber);

            return builder.Build();
        }
    }
}
