using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers.Areas.Moderator
{
    [Route("api/moderator/auction-categories")]
    [Authorize(Roles = UserRoles.Moderator)]
    public class AuctionCategoriesController : BaseController
    {
        private readonly IAuctionCategoriesService _service;

        public AuctionCategoriesController(IAuctionCategoriesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] SpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<AuctionCategoryDTO>> result = await _service
                .GetAuctionCategoriesAsync(specifications);

            return FromResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] UpsertAuctionCategoryDTO categoryDTO)
        {
            ServiceResult result = await _service.CreateAuctionCategoryAsync(categoryDTO);

            return FromResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CreateCategory([FromRoute] long id, 
            [FromBody] UpsertAuctionCategoryDTO categoryDTO)
        {
            ServiceResult result = await _service.UpdateAuctionCategoryAsync(id, categoryDTO);

            return FromResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] long id)
        {
            ServiceResult result = await _service.DeleteAuctionCategoryAsync(id);

            return FromResult(result);
        }
    }
}
