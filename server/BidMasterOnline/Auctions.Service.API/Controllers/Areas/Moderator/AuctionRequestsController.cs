using Auctions.Service.API.DTO;
using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers.Areas.Moderator;

[Route("api/moderators/auctions/requests")]
[Authorize(Roles = UserRoles.Moderator)]
public class AuctionRequestsController : BaseController
{
    private readonly IModeratorAuctionRequestsService _service;

    public AuctionRequestsController(IModeratorAuctionRequestsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllActionResults([FromQuery] AuctionRequestSpecificationsDTO specifications)
    {
        ServiceResult<PaginatedList<AuctionRequestSummaryDTO>> result = await _service.GetAllAuctionRequestAsync(specifications);

        return FromResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuctionRequestById([FromRoute] long id)
    {
        ServiceResult<AuctionRequestDTO> result = await _service.GetAuctionRequestById(id);

        return FromResult(result);
    }
}
