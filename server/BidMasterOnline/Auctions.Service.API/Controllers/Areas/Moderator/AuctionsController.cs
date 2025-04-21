using Auctions.Service.API.DTO;
using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers.Areas.Moderator;

[Route("api/moderators/[controller]")]
[ApiController]
[Authorize(Roles = UserRoles.Moderator)]
public class AuctionsController : BaseController
{
    private readonly IModeratorAuctionsService _service;

    public AuctionsController(IModeratorAuctionsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAuctions([FromQuery] AuctionSpecificationsDTO specifications)
    {
        ServiceResult<PaginatedList<AuctionSummaryDTO>> result = await _service.GetAuctionsListAsync(specifications);

        return FromResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAllAuctions([FromRoute] long id)
    {
        ServiceResult<AuctionDTO> result = await _service.GetAuctionByIdAsync(id);

        return FromResult(result);
    }
}
