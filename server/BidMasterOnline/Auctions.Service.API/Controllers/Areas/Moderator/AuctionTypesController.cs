using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers.Areas.Moderator
{
    [Route("api/moderator/auction-types")]
    [Authorize(Roles = UserRoles.Moderator)]
    public class AuctionTypesController : BaseController
    {
        private readonly IAuctionTypesService _service;

        public AuctionTypesController(IAuctionTypesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetTypesList([FromQuery] SpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<AuctionTypeDTO>> result = await _service
                .GetAuctionTypesListAsync(specifications);

            return FromResult(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTypes()
        {
            ServiceResult<IEnumerable<AuctionTypeDTO>> result = await _service
                .GetAllAuctionTypesAsync();

            return FromResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CreateType([FromRoute] long id,
            [FromBody] UpdateAuctionTypeDTO typeDTO)
        {
            ServiceResult result = await _service.UpdateAuctionTypeAsync(id, typeDTO);

            return FromResult(result);
        }
    }
}
