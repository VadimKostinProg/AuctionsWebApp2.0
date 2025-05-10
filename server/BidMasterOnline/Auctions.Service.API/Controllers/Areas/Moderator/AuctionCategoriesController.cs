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
        private readonly IModeratorAuctionCategoriesService _service;

        public AuctionCategoriesController(IModeratorAuctionCategoriesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] ModeratorSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<ModeratorAuctionCategoryDTO>> result = await _service
                .GetAuctionCategoriesAsync(specifications);

            return FromResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] ModeratorUpsertAuctionCategoryDTO categoryDTO)
        {
            ServiceResult result = await _service.CreateAuctionCategoryAsync(categoryDTO);

            return FromResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CreateCategory([FromRoute] long id, 
            [FromBody] ModeratorUpsertAuctionCategoryDTO categoryDTO)
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
