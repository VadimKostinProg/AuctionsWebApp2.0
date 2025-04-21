using Auctions.Service.API.DTO;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Entities;
using BidMasterOnline.Domain.Models;
using System;

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

        public async Task<ServiceResult<ListModel<AuctionSummaryDTO>>> GetAuctionsListAsync(AuctionSpecificationsDTO specifications)
        {
            ServiceResult<ListModel<AuctionSummaryDTO>> result = new();

            ISpecification<Auction> specification = GetSpecification(specifications);

            ListModel<Auction> auctionsList = await _repository.GetFilteredAndPaginated(specification);

            result.Data = new ListModel<AuctionSummaryDTO>
            {
                Items = auctionsList.Items.Select(x => x.ToSummaryDTO()).ToList(),
                Pagination = auctionsList.Pagination
            };

            return result;
        }

        public async Task<ServiceResult<AuctionDTO>> GetAuctionByIdAsync(long id)
        {
            ServiceResult<AuctionDTO> result = new();

            Auction? auction = await _repository.GetFirstOrDefaultAsync<Auction>(x => x.Id == id);

            if (auction == null)
            {
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
                result.Errors.Add("Auction not found.");

                return result;
            }

            result.Data = auction.ToDTO();

            return result;
        }

        public async Task<ServiceResult> CancelAuctionAsync(long id)
        {
            ServiceResult result = new();

            long userId = _userAccessor.UserId;

            Auction? auction = await _repository.GetFirstOrDefaultAsync<Auction>(x => x.Id == id && x.AuctionistId == userId);

            if (auction == null)
            {
                result.StatusCode = System.Net.HttpStatusCode.NotFound;
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
