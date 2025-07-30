using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.Constants;
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
        private readonly INotificationsService _notificationsService;

        public AuctionsService(IRepository repository,
            IUserAccessor userAccessor,
            ITransactionsService transactionsService,
            ILogger<AuctionsService> logger,
            INotificationsService notificationsService)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _transactionsService = transactionsService;
            _logger = logger;
            _notificationsService = notificationsService;
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

        public async Task<ServiceResult<IEnumerable<AuctionSummaryDTO>>> GetNotDeliveredAuctionsForBuyerAsync()
        {
            ServiceResult<IEnumerable<AuctionSummaryDTO>> result = new();

            long userId = _userAccessor.UserId;

            List<Auction> auctions = await _repository.GetFiltered<Auction>(a => 
                    (a.Status == AuctionStatus.Finished && a.Type!.Name == AuctionTypes.DutchAuction && a.AuctioneerId == userId && !a.IsDeliveryPerformed) ||
                    (a.Status == AuctionStatus.Finished && a.Type!.Name != AuctionTypes.DutchAuction && a.WinnerId == userId && !a.IsDeliveryPerformed),
                    includeQuery: query => query.Include(a => a.Type)!)
                .OrderBy(a => a.FinishTime)
                .ToListAsync();

            result.Data = auctions.Select(a => a.ToParticipantSummaryDTO());

            return result;
        }

        public async Task<ServiceResult<IEnumerable<AuctionSummaryDTO>>> GetNotPayedAuctionsForSellerAsync()
        {
            ServiceResult<IEnumerable<AuctionSummaryDTO>> result = new();

            long userId = _userAccessor.UserId;

            List<Auction> auctions = await _repository.GetFiltered<Auction>(a =>
                    (a.Status == AuctionStatus.Finished && a.Type!.Name == AuctionTypes.DutchAuction && a.WinnerId == userId && !a.IsPaymentPerformed) ||
                    (a.Status == AuctionStatus.Finished && a.Type!.Name != AuctionTypes.DutchAuction && a.AuctioneerId == userId && !a.IsPaymentPerformed),
                    includeQuery: query => query.Include(a => a.Type)!)
                .OrderBy(a => a.FinishTime)
                .ToListAsync();

            result.Data = auctions.Select(a => a.ToParticipantSummaryDTO());

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
                includeQuery: query => query.Include(e => e.Type)
                                            .Include(e => e.Bids)!);

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

                    await _notificationsService.SendMessageOfFinishingAuctionToBuyer(auction);
                    await _notificationsService.SendMessageOfFinishingAuctionToSeller(auction);
                }
                else
                {
                    await _notificationsService.SendMessageOfNoWinnersOfAuctionToAuctioneer(auction);
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

        public async Task<bool> StartPendingAuctionAsync(long id)
        {
            Auction auction = await _repository.GetByIdAsync<Auction>(id);

            try
            {
                auction.Status = AuctionStatus.Active;

                await _notificationsService.SendMessageOfStartingAuctionToAuctioneer(auction);

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

        public async Task<ServiceResult> SetDeliveryWaybillForAuctionAsync(SetDeliveryWaybillDTO request)
        {
            ServiceResult result = new();

            long userId = _userAccessor.UserId;

            Auction? auction = await _repository.GetFirstOrDefaultAsync<Auction>(e => e.Id == request.AuctionId,
                includeQuery: query => query.Include(e => e.Type)!);

            if (auction == null)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("Auction not found");

                return result;
            }

            if (!CheckSellerForAuction(auction, userId))
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add("Only seller of this auction are allowed to apply delivery of it");

                return result;
            }

            if (auction.Status != AuctionStatus.Finished)
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Errors.Add("Auction is not finished yet");

                return result;
            }

            try
            {
                auction.IsDeliveryPerformed = true;
                auction.DeliveryPerformedTime = DateTime.Now;
                auction.DeliveryWaybill = request.Waybill;

                _repository.Update(auction);
                await _repository.SaveChangesAsync();

                await _notificationsService.SendMessageOfPerformingDeliveryToBuyer(auction);

                result.Message = "Your waybill has been successfully saved.";
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occured while saving waybill for auction.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured while saving waybill for auction.");
            }

            return result;
        }

        private bool CheckSellerForAuction(Auction auction, long sellerId)
            => (auction.Type!.Name == AuctionTypes.DutchAuction && auction.WinnerId == sellerId) ||
               (auction.Type!.Name != AuctionTypes.DutchAuction && auction.AuctioneerId == sellerId);

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

            if (!string.IsNullOrEmpty(specifications.SortBy))
            {
                switch (specifications.SortBy)
                {
                    case "name":
                        builder.OrderBy(x => x.LotTitle, specifications.SortDirection);
                        break;
                    case "currentPrice":
                        builder.OrderBy(x => x.CurrentPrice, specifications.SortDirection);
                        break;
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
