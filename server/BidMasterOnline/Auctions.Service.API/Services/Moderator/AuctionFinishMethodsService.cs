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
    public class AuctionFinishMethodsService : IAuctionFinishMethodsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<AuctionFinishMethodsService> _logger;

        public AuctionFinishMethodsService(IRepository repository, 
            ILogger<AuctionFinishMethodsService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ServiceResult<PaginatedList<DTO.Moderator.AuctionFinishMethodDTO>>> GetAuctionFinishMethodsAsync(SpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<DTO.Moderator.AuctionFinishMethodDTO>> result = new();

            SpecificationBuilder<AuctionFinishMethod> specificationBuilder = new();

            if (!string.IsNullOrEmpty(specifications.SearchTerm))
                specificationBuilder.With(e => e.Name.Contains(specifications.SearchTerm) ||
                                               e.Description.Contains(specifications.SearchTerm));

            if (!specifications.IncludeDeleted)
                specificationBuilder.With(e => !e.Deleted);

            specificationBuilder.WithPagination(specifications.PageSize, specifications.PageNumber);

            ListModel<AuctionFinishMethod> entitiesList = await _repository.GetFilteredAndPaginated(
                specificationBuilder.Build());

            result.Data = entitiesList.ToPaginatedList(e => e.ToModeratorDTO());

            return result;
        }

        public async Task<ServiceResult> UpdateAuctionFinishMethodAsync(long id, UpdateAuctionFinishMethodDTO auctionFinishMethodDTO)
        {
            ServiceResult result = new();

            try
            {
                AuctionFinishMethod entity = await _repository.GetByIdAsync<AuctionFinishMethod>(id);

                entity.Description = auctionFinishMethodDTO.Description;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                result.Message = "Auction finish method has been updated successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured during updating auction finish method.");

                result.IsSuccessfull = false;
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Errors.Add("An error occured during updating auction finish method.");
            }

            return result;
        }
    }
}
