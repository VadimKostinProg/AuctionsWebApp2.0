using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Bids.Service.API.DTO.Moderator;
using Bids.Service.API.Extensions;
using Bids.Service.API.ServiceContracts.Moderator;
using Microsoft.EntityFrameworkCore;

namespace Bids.Service.API.Services.Moderator
{
    public class BidsService : IBidsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<BidsService> _logger;

        public BidsService(IRepository repository, ILogger<BidsService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> ClearAllBidsForAuctionAsync(long auctionId)
        {
            try
            {
                List<Bid> auctionBids = await _repository.GetFiltered<Bid>(e => e.AuctionId == auctionId)
                    .ToListAsync();

                auctionBids.ForEach(b => b.Deleted = true);

                await _repository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured during clearing all bids of auction {auctionId}.");

                return false;
            }
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

                result.Data = bidsList.ToPaginatedList(e => e.ToModeratorAuctionBidDTO());
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

        public async Task<ServiceResult<PaginatedList<UserBidDTO>>> GetUserBidsAsync(long userId, 
            PaginationRequestDTO pagination)
        {
            ServiceResult<PaginatedList<UserBidDTO>> result = new();

            try
            {
                ISpecification<Bid> specification = new SpecificationBuilder<Bid>()
                    .With(e => e.BidderId == userId)
                    .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                    .WithPagination(pagination.PageSize, pagination.PageNumber)
                    .Build();

                ListModel<Bid> bidsList = await _repository.GetFilteredAndPaginated(specification,
                    includeQuery: query => query.Include(e => e.Auction)!);

                result.Data = bidsList.ToPaginatedList(e => e.ToModeratorUserBidDTO());
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
    }
}
