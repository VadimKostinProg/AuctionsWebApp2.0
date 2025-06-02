using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Bids.Service.API.DTO.Participant;
using Bids.Service.API.Extensions;
using Bids.Service.API.GrpcServices.Client;
using Bids.Service.API.ServiceContracts.Participant;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Bids.Service.API.Services.Participant
{
    public class BidsService : IBidsService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly IBidsPlacingStrategyFactory _bidsPlacingStrategyFactory;
        private readonly IUserStatusValidationService _userStatusValidationService;
        private readonly ILogger<BidsService> _logger;
        private readonly AuctionsGrpcClient _auctionsClient;

        private static readonly ConcurrentDictionary<long, SemaphoreSlim> _auctionLocks = [];

        public BidsService(IRepository repository,
            IUserAccessor userAccessor,
            IBidsPlacingStrategyFactory bidsPlacingStrategyFactory,
            IUserStatusValidationService userStatusValidationService,
            ILogger<BidsService> logger,
            AuctionsGrpcClient auctionsClient)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _bidsPlacingStrategyFactory = bidsPlacingStrategyFactory;
            _userStatusValidationService = userStatusValidationService;
            _logger = logger;
            _auctionsClient = auctionsClient;
        }

        public async Task<ServiceResult<PaginatedList<AuctionBidDTO>>> GetAuctionBidsAsync(long auctionId,
            PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<AuctionBidDTO>> result = new();

            try
            {
                ISpecification<Bid> specification = new SpecificationBuilder<Bid>()
                    .With(e => e.AuctionId == auctionId && !e.Deleted)
                    .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                    .WithPagination(pagination.PageSize, pagination.PageNumber)
                    .Build();

                ListModel<Bid> bidsList = await _repository.GetFilteredAndPaginated(specification,
                    includeQuery: query => query.Include(e => e.Bidder)!);

                result.Data = bidsList.ToPaginatedList(e => e.ToParticipantAuctionBidDTO());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during fetching the bids.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during fetching the bids.");
            }

            return result;
        }

        public async Task<ServiceResult<PaginatedList<UserBidDTO>>> GetUserBidsAsync(PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<UserBidDTO>> result = new();

            try
            {
                long userId = _userAccessor.UserId;

                ISpecification<Bid> specification = new SpecificationBuilder<Bid>()
                    .With(e => e.BidderId == userId)
                    .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                    .WithPagination(pagination.PageSize, pagination.PageNumber)
                    .Build();

                ListModel<Bid> bidsList = await _repository.GetFilteredAndPaginated(specification,
                    includeQuery: query => query.Include(e => e.Auction)!);

                result.Data = bidsList.ToPaginatedList(e => e.ToParticipantUserBidDTO());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during fetching the bids.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during fetching the bids.");
            }

            return result;
        }

        public async Task<ServiceResult> PostBidOnAuctionAsync(PostBidDTO bidDTO)
        {
            ServiceResult result = new();

            if (!await _userStatusValidationService.IsActiveAsync())
            {
                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.Forbidden;
                result.Errors.Add("You have no rights to place bids on auctions for now.");

                return result;
            }

            SemaphoreSlim semaphore = _auctionLocks.GetOrAdd(bidDTO.AuctionId,
                _ => new SemaphoreSlim(initialCount: 1, maxCount: 1));

            await semaphore.WaitAsync();

            try
            {
                Auction? auction = await _repository.GetFirstOrDefaultAsync<Auction>(e => e.Id == bidDTO.AuctionId,
                    includeQuery: query => query.Include(e => e.Bids)
                                                .Include(e => e.Type)
                                                .Include(e => e.FinishMethod)!);

                if (auction == null)
                {
                    result.IsSuccessfull = false;
                    result.StatusCode = System.Net.HttpStatusCode.NotFound;
                    result.Errors.Add("Auction not fount.");

                    return result;
                }

                Bid newBid = bidDTO.ToDomain();
                newBid.BidderId = _userAccessor.UserId;

                IBidsPlacingStrategy strategy = _bidsPlacingStrategyFactory.GetStategyByAuctionType(auction.Type!);

                ServiceResult placingResult = strategy.PlaceNewBid(newBid, auction);

                if (!placingResult.IsSuccessfull)
                    return placingResult;

                await _repository.AddAsync(newBid);
                _repository.Update(auction);
                await _repository.SaveChangesAsync();

                if (auction.Status == BidMasterOnline.Domain.Enums.AuctionStatus.Finished)
                {
                    await _auctionsClient.FinishAuctionAsync(auction.Id);
                }

                result.Message = "Your bid has been placed successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during placing the bid.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during placing the bid.");
            }
            finally
            {
                semaphore.Release();
            }

            return result;
        }
    }
}
