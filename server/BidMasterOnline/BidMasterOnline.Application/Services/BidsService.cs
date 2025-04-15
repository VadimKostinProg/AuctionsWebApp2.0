using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Exceptions;
using BidMasterOnline.Application.RepositoryContracts;
using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Application.Specifications;
using BidMasterOnline.Domain.Entities;

namespace BidMasterOnline.Application.Services
{
    public class BidsService : IBidsService
    {
        private readonly IRepository _repository;
        private readonly IAuthService _authService;

        public BidsService(IRepository repository, IAuthService jwtService)
        {
            _repository = repository;
            _authService = jwtService;
        }

        public async Task<ListModel<BidDTO>> GetBidsListForAuctionAsync(Guid auctionId, SpecificationsDTO specifications)
        {
            if (specifications is null)
                throw new ArgumentNullException("Specifications are null.");

            if (!await _repository.AnyAsync<Auction>(x => x.Id == auctionId))
                throw new KeyNotFoundException("Auction with such id does not exist.");

            var specification = new SpecificationBuilder<Bid>()
                .With(x => x.AuctionId == auctionId)
                .OrderBy(x => x.DateAndTime, Enums.SortDirection.DESC)
                .WithPagination(specifications.PageSize, specifications.PageNumber)
                .Build();

            var bids = await _repository.GetAsync<Bid>(specification);

            var totalCount = await _repository.CountAsync<Bid>(x => x.AuctionId == auctionId);

            var totalPages = (long)Math.Ceiling((double) totalCount / specifications.PageSize);

            var list = new ListModel<BidDTO>
            {
                List = bids.Select(x => new BidDTO
                {
                    Id = x.Id,
                    AuctionId = x.AuctionId,
                    AuctionName = x.Auction.Name,
                    BidderId = x.BidderId,
                    BidderUsername = x.Bidder.Username,
                    DateAndTime = x.DateAndTime.ToString("yyyy-MM-dd HH:m"),
                    Amount = x.Amount,
                    IsWinning = x.IsWinning
                })
                .ToList(),
                TotalPages = totalPages
            };

            return list;
        }

        public async Task<ListModel<BidDTO>> GetBidsListForUserAsync(Guid userId, SpecificationsDTO specifications)
        {
            if (specifications is null)
                throw new ArgumentNullException("Specifications are null.");

            if (!await _repository.AnyAsync<User>(x => x.Id == userId))
                throw new KeyNotFoundException("User with such id does not exist.");

            var specificationBuilder = new SpecificationBuilder<Bid>()
                .With(x => x.BidderId == userId)
                .OrderBy(x => x.DateAndTime, Enums.SortDirection.DESC)
                .WithPagination(specifications.PageSize, specifications.PageNumber);

            var specification = specificationBuilder.Build();

            var bids = await _repository.GetAsync<Bid>(specification);

            var totalCount = await _repository.CountAsync<Bid>(specification.Predicate);

            var totalPages = (long)Math.Ceiling((double)totalCount / specifications.PageSize);

            var list = new ListModel<BidDTO>
            {
                List = bids.Select(x => new BidDTO
                {
                    Id = x.Id,
                    AuctionId = x.AuctionId,
                    AuctionName = x.Auction.Name,
                    BidderId = x.BidderId,
                    BidderUsername = x.Bidder.Username,
                    DateAndTime = x.DateAndTime.ToString("yyyy-MM-dd HH:m"),
                    Amount = x.Amount,
                    IsWinning = x.IsWinning
                })
                .ToList(),
                TotalPages = totalPages
            };

            return list;
        }

        public async Task SetBidAsync(SetBidDTO bid)
        {
            // Validating bid
            if (bid is null)
                throw new ArgumentNullException("Bid is null.");

            var user = await _authService.GetAuthenticatedUserEntityAsync();

            if (user.UserStatus.Name == Enums.UserStatus.Blocked.ToString())
                throw new ForbiddenException("Account is blocked.");

            if (!user.IsEmailConfirmed)
                throw new ForbiddenException("Your email is not confirmed.");

            var auction = await _repository.GetByIdAsync<Auction>(bid.AuctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.FinishDateTime <= DateTime.UtcNow)
                throw new InvalidOperationException("Auction has already finished.");

            if (auction.Status.Name == Enums.AuctionStatus.Canceled.ToString())
                throw new InvalidOperationException("Auction is canceled.");

            if (auction.AuctionistId == user.Id)
                throw new InvalidOperationException("Cannot set a bid as an acutionist.");

            var maxBid = auction.Bids.OrderByDescending(x => x.Amount).FirstOrDefault();

            if (maxBid is not null && maxBid.Amount > bid.Amount)
                throw new ArgumentException("Bid is less than previous one.");
            else if (maxBid is not null && maxBid.BidderId == user.Id)
                throw new InvalidOperationException("You have already placed the last bid at this auction.");
            else if (auction.StartPrice > bid.Amount)
                throw new ArgumentException("Bid is less than start price.");

            // Setting new bid
            var bidToSet = new Bid
            {
                AuctionId = bid.AuctionId,
                BidderId = user.Id,
                DateAndTime = DateTime.UtcNow,
                Amount = bid.Amount,
            };

            await _repository.AddAsync(bidToSet);

            if (auction.FinishType.Name == Enums.AuctionFinishType.IncreasingFinishTime.ToString())
            {
                auction.FinishDateTime = 
                    auction.FinishDateTime.Add(new TimeSpan(auction.FinishInterval!.Value));

                await _repository.UpdateAsync(auction);
            }
        }
    }
}
