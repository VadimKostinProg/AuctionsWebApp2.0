using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auctions.Service.API.Services.Moderator
{
    public class AuctionTypesService : IAuctionTypesService
    {
        private readonly IRepository _repository;
        private readonly ILogger<AuctionTypesService> _logger;

        public AuctionTypesService(IRepository repository, 
            ILogger<AuctionTypesService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<AuctionTypeDTO>>> GetAllAuctionTypesAsync()
        {
            ServiceResult<IEnumerable<AuctionTypeDTO>> result = new();

            List<AuctionType> entities = await _repository.GetFiltered<AuctionType>(e => !e.Deleted)
                .OrderBy(e => e.Name)
                .ToListAsync();

            result.Data = entities.Select(e => e.ToModeratorDTO());

            return result;
        }

        public async Task<ServiceResult<PaginatedList<AuctionTypeDTO>>> GetAuctionTypesListAsync(
            SpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<AuctionTypeDTO>> result = new();

            SpecificationBuilder<AuctionType> specificationBuilder = new();

            if (!string.IsNullOrEmpty(specifications.SearchTerm))
                specificationBuilder.With(e => e.Name.Contains(specifications.SearchTerm) ||
                                               e.Description.Contains(specifications.SearchTerm));

            if (!specifications.IncludeDeleted)
                specificationBuilder.With(e => !e.Deleted);

            specificationBuilder.WithPagination(specifications.PageSize, specifications.PageNumber);

            ListModel<AuctionType> entitiesList = await _repository.GetFilteredAndPaginated(
                specificationBuilder.Build());

            result.Data = entitiesList.ToPaginatedList(e => e.ToModeratorDTO());

            return result;
        }

        public async Task<ServiceResult> UpdateAuctionTypeAsync(long id, UpdateAuctionTypeDTO auctionTypeDTO)
        {
            ServiceResult result = new();

            try
            {
                AuctionType entity = await _repository.GetByIdAsync<AuctionType>(id);

                entity.Description = auctionTypeDTO.Description;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                result.Message = "Auction type has been updated successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during updating auction type.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during updating auction type.");
            }

            return result;
        }
    }
}
