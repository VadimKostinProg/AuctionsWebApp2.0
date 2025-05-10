using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers.Areas.Moderator
{
    [Route("api/moderator/auction-finish-methods")]
    [Authorize(Roles = UserRoles.Moderator)]
    public class AuctionFinishMethodsController : BaseController
    {
        private readonly IModeratorAuctionFinishMethodsService _service;

        public AuctionFinishMethodsController(IModeratorAuctionFinishMethodsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetFinishMethods([FromQuery] ModeratorSpecificationsDTO specifications)
        {
            ServiceResult<PaginatedList<ModeratorAuctionFinishMethodDTO>> result = await _service
                .GetAuctionFinishMethodsAsync(specifications);

            return FromResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> CreateFinishMethod([FromRoute] long id,
            [FromBody] ModeratorUpdateAuctionFinishMethodDTO finishMethodDTO)
        {
            ServiceResult result = await _service.UpdateAuctionFinishMethodAsync(id, finishMethodDTO);

            return FromResult(result);
        }
    }
}
