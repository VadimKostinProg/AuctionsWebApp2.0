using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;

namespace Auctions.Service.API.Services.Moderator
{
    public class ModeratorAuctionCategoriesService : IModeratorAuctionCategoriesService
    {
        private readonly IRepository _repository;
        private readonly ILogger<ModeratorAuctionCategoriesService> _logger;

        public ModeratorAuctionCategoriesService(IRepository repository, 
            ILogger<ModeratorAuctionCategoriesService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ServiceResult> CreateAuctionCategoryAsync(ModeratorUpsertAuctionCategoryDTO auctionCategoryDTO)
        {
            ServiceResult result = new();

            try
            {
                AuctionCategory entity = new()
                {
                    Name = auctionCategoryDTO.Name,
                    Description = auctionCategoryDTO.Description
                };
                    
                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during creating auction category.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during creating auction category.");
            }

            return result;
        }

        public async Task<ServiceResult> DeleteAuctionCategoryAsync(long id)
        {
            ServiceResult result = new();

            try
            {
                await _repository.UpdateManyAsync<AuctionCategory>(e => e.Id == id,
                    e => e.Deleted, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during deleting auction category.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during deleting auction category.");
            }

            return result;
        }

        public async Task<ServiceResult<PaginatedList<ModeratorAuctionCategoryDTO>>> GetAuctionCategoriesAsync(ModeratorSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<ModeratorAuctionCategoryDTO>> result = new();

            SpecificationBuilder<AuctionCategory> specificationBuilder = new();

            if (!string.IsNullOrEmpty(specifications.Search))
                specificationBuilder.With(e => e.Name.Contains(specifications.Search) ||
                                               e.Description.Contains(specifications.Search));

            if (!specifications.IncludeDeleted)
                specificationBuilder.With(e => !e.Deleted);

            specificationBuilder.WithPagination(specifications.PageSize, specifications.PageNumber);

            ListModel<AuctionCategory> entitiesList = await _repository.GetFilteredAndPaginated(
                specificationBuilder.Build());

            result.Data = entitiesList.ToPaginatedList(e => e.ToModeratorDTO());

            return result;
        }

        public async Task<ServiceResult> UpdateAuctionCategoryAsync(long id, ModeratorUpsertAuctionCategoryDTO auctionCategoryDTO)
        {
            ServiceResult result = new();

            try
            {
                AuctionCategory entity = await _repository.GetByIdAsync<AuctionCategory>(id);

                entity.Name = auctionCategoryDTO.Name;
                entity.Description = auctionCategoryDTO.Description;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during updating auction category.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during updating auction category.");
            }

            return result;
        }
    }
}
