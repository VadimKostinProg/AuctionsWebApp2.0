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
    public class ModeratorAuctionFinishMethodsService : IModeratorAuctionFinishMethodsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<ModeratorAuctionFinishMethodsService> _logger;

        public ModeratorAuctionFinishMethodsService(IRepository repository, 
            ILogger<ModeratorAuctionFinishMethodsService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ServiceResult<PaginatedList<ModeratorAuctionFinishMethodDTO>>> GetAuctionFinishMethodsAsync(ModeratorSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<ModeratorAuctionFinishMethodDTO>> result = new();

            SpecificationBuilder<AuctionFinishMethod> specificationBuilder = new();

            if (!string.IsNullOrEmpty(specifications.Search))
                specificationBuilder.With(e => e.Name.Contains(specifications.Search) ||
                                               e.Description.Contains(specifications.Search));

            if (!specifications.IncludeDeleted)
                specificationBuilder.With(e => !e.Deleted);

            specificationBuilder.WithPagination(specifications.PageSize, specifications.PageNumber);

            ListModel<AuctionFinishMethod> entitiesList = await _repository.GetFilteredAndPaginated(
                specificationBuilder.Build());

            result.Data = entitiesList.ToPaginatedList(e => e.ToModeratorDTO());

            return result;
        }

        public async Task<ServiceResult> UpdateAuctionFinishMethodAsync(long id, ModeratorUpdateAuctionFinishMethodDTO auctionFinishMethodDTO)
        {
            ServiceResult result = new();

            try
            {
                AuctionFinishMethod entity = await _repository.GetByIdAsync<AuctionFinishMethod>(id);

                entity.Description = auctionFinishMethodDTO.Description;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();
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
