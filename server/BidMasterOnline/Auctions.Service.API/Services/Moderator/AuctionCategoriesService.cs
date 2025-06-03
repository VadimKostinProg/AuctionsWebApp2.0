using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.Extensions;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Enums;
using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.Specifications;
using BidMasterOnline.Domain.Models;
using BidMasterOnline.Domain.Models.Entities;

namespace Auctions.Service.API.Services.Moderator
{
    public class AuctionCategoriesService : IAuctionCategoriesService
    {
        private readonly IRepository _repository;
        private readonly ILogger<AuctionCategoriesService> _logger;

        public AuctionCategoriesService(IRepository repository, 
            ILogger<AuctionCategoriesService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ServiceResult> CreateAuctionCategoryAsync(UpsertAuctionCategoryDTO auctionCategoryDTO)
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

                result.Message = "Category has been created successfully.";
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
                AuctionCategory category = await _repository.GetByIdAsync<AuctionCategory>(id);

                category.Deleted = true;

                _repository.Update(category);
                await _repository.SaveChangesAsync();

                result.Message = "Category has been deleted successfully.";
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

        public async Task<ServiceResult<PaginatedList<DTO.Moderator.AuctionCategoryDTO>>> GetAuctionCategoriesAsync(SpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<DTO.Moderator.AuctionCategoryDTO>> result = new();

            SpecificationBuilder<AuctionCategory> specificationBuilder = new();

            if (!string.IsNullOrEmpty(specifications.SearchTerm))
                specificationBuilder.With(e => e.Name.Contains(specifications.SearchTerm) ||
                                               e.Description.Contains(specifications.SearchTerm));

            if (!specifications.IncludeDeleted)
                specificationBuilder.With(e => !e.Deleted);

            if (!string.IsNullOrEmpty(specifications.SortBy))
                switch(specifications.SortBy)
                {
                    case "id":
                        specificationBuilder.OrderBy(e => e.Id, specifications.SortDirection ?? SortDirection.ASC);
                        break;
                    case "name":
                        specificationBuilder.OrderBy(e => e.Name, specifications.SortDirection ?? SortDirection.ASC);
                        break;
                }

            specificationBuilder.WithPagination(specifications.PageSize, specifications.PageNumber);

            ListModel<AuctionCategory> entitiesList = await _repository.GetFilteredAndPaginated(
                specificationBuilder.Build());

            result.Data = entitiesList.ToPaginatedList(e => e.ToModeratorDTO());

            return result;
        }

        public async Task<ServiceResult> UpdateAuctionCategoryAsync(long id, UpsertAuctionCategoryDTO auctionCategoryDTO)
        {
            ServiceResult result = new();

            try
            {
                AuctionCategory entity = await _repository.GetByIdAsync<AuctionCategory>(id);

                entity.Name = auctionCategoryDTO.Name;
                entity.Description = auctionCategoryDTO.Description;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                result.Message = "Category has been updated successfully.";
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
