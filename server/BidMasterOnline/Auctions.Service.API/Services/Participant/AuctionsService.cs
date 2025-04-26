using Auctions.Service.API.DTO;
using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Service.API.Services.Participant
{
    public class AuctionsService : IAuctionsService
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;

        public AuctionsService(IRepository repository,
            IUserAccessor userAccessor)
        {
            _repository = repository;
            _userAccessor = userAccessor;
        }

        public async Task<ServiceResult<PaginatedList<AuctionSummaryDTO>>> GetAuctionsListAsync(AuctionSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<AuctionSummaryDTO>> result = new();

            ISpecification<Auction> specification = GetSpecification(specifications);

            ListModel<Auction> auctionsList = await _repository.GetFilteredAndPaginated(specification,
                includeQuery: query => query.Include(e => e.Auctionist)
                                            .Include(e => e.Images)!);

            result.Data = new PaginatedList<AuctionSummaryDTO>
            {
                Items = auctionsList.Items.Select(x => x.ToParticipantSummaryDTO()).ToList(),
                Pagination = new()
                {
                    TotalCount = auctionsList.TotalCount,
                    TotalPages = auctionsList.TotalPages,
                    CurrentPage = auctionsList.CurrentPage,
                    PageSize = auctionsList.PageSize
                }
            };

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

        public virtual async Task<ServiceResult> CancelAuctionAsync(long id)
        {
            ServiceResult result = new();

            long userId = _userAccessor.UserId;

            Auction? auction = await _repository.GetFirstOrDefaultAsync<Auction>(x => x.Id == id && x.AuctionistId == userId);

            if (auction == null)
            {
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.IsSuccessfull = false;
                result.Errors.Add("Auction not found.");

                return result;
            }

            auction.Status = BidMasterOnline.Domain.Enums.AuctionStatus.CancelledByAuctionist;
            _repository.Update(auction);

            await _repository.SaveChangesAsync();

            result.Message = "Auction has been cancelled successfully!";

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
