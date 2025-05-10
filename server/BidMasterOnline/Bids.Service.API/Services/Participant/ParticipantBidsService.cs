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

namespace Bids.Service.API.Services.Participant
{
    public class ParticipantBidsService : IParticipantBidsService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly IBidsPlacingStrategyFactory _bidsPlacingStrategyFactory;
        private readonly ILogger<ParticipantBidsService> _logger;
        private readonly AuctionsGrpcClient _auctionsClient;

        public ParticipantBidsService(IRepository repository,
            IUserAccessor userAccessor,
            IBidsPlacingStrategyFactory bidsPlacingStrategyFactory,
            ILogger<ParticipantBidsService> logger,
            AuctionsGrpcClient auctionsClient)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _logger = logger;
            _bidsPlacingStrategyFactory = bidsPlacingStrategyFactory;
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
                    .Build();

                ListModel<Bid> bidsList = await _repository.GetFilteredAndPaginated(specification);

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
                    .Build();

                ListModel<Bid> bidsList = await _repository.GetFilteredAndPaginated(specification);

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

                IBidsPlacingStrategy strategy = _bidsPlacingStrategyFactory.GetStategyByAuctionType(auction.Type!);

                ServiceResult placingResult = strategy.PlaceNewBid(newBid, auction);

                if (!placingResult.IsSuccessfull)
                    return placingResult;

                _repository.Update(newBid);
                _repository.Update(auction);
                await _repository.SaveChangesAsync();

                await _auctionsClient.FinishAuctionAsync(auction.Id);

                result.Message = "Your bid has been placed successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during placing the bid.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during placing the bid.");
            }

            return result;
        }
    }
}
