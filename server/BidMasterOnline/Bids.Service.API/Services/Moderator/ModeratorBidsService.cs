using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Bids.Service.API.DTO.Moderator;
using Bids.Service.API.Extensions;
using Bids.Service.API.ServiceContracts.Moderator;

namespace Bids.Service.API.Services.Moderator
{
    public class ModeratorBidsService : IModeratorBidsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<ModeratorBidsService> _logger;

        public ModeratorBidsService(IRepository repository, ILogger<ModeratorBidsService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> ClearAllBidsForAuctionAsync(long auctionId)
        {
            try
            {
                await _repository.UpdateManyAsync<Bid>(e => e.AuctionId == auctionId, e => e.Deleted, true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured during clearing all bids of auction {auctionId}.");

                return false;
            }
        }

        public async Task<ServiceResult<PaginatedList<AuctionBidDTO>>> GetAuctionBidsAsync(long auctionId)
        {
            ServiceResult<PaginatedList<AuctionBidDTO>> result = new();

            try
            {
                ISpecification<Bid> specification = new SpecificationBuilder<Bid>()
                    .With(e => e.AuctionId == auctionId)
                    .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                    .Build();

                ListModel<Bid> bidsList = await _repository.GetFilteredAndPaginated(specification);

                result.Data = new PaginatedList<AuctionBidDTO>
                {
                    Items = bidsList.Items.Select(e => e.ToModeratorAuctionBidDTO()).ToList(),
                    Pagination = new Pagination
                    {
                        TotalCount = bidsList.TotalCount,
                        TotalPages = bidsList.TotalPages,
                        CurrentPage = bidsList.CurrentPage,
                        PageSize = bidsList.PageSize
                    }
                };
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

        public async Task<ServiceResult<PaginatedList<UserBidDTO>>> GetUserBidsAsync(long userId)
        {
            ServiceResult<PaginatedList<UserBidDTO>> result = new();

            try
            {
                ISpecification<Bid> specification = new SpecificationBuilder<Bid>()
                    .With(e => e.BidderId == userId)
                    .OrderBy(e => e.CreatedAt, BidMasterOnline.Core.Enums.SortDirection.DESC)
                    .Build();

                ListModel<Bid> bidsList = await _repository.GetFilteredAndPaginated(specification);

                result.Data = new PaginatedList<UserBidDTO>
                {
                    Items = bidsList.Items.Select(e => e.ToModeratorUserBidDTO()).ToList(),
                    Pagination = new Pagination
                    {
                        TotalCount = bidsList.TotalCount,
                        TotalPages = bidsList.TotalPages,
                        CurrentPage = bidsList.CurrentPage,
                        PageSize = bidsList.PageSize
                    }
                };
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
