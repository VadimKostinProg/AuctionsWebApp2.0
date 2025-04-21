using Auctions.Service.API.DTO;
using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers.Areas.Participant;

[Route("api/participants/[controller]")]
[ApiController]
[Authorize(Roles = UserRoles.Participant)]
public class AuctionsController : BaseController
{
    private readonly IAuctionsService _service;

    public AuctionsController(IAuctionsService service)
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
    public async Task<IActionResult> GetAuctionById([FromRoute] long id)
    {
        ServiceResult<AuctionDTO> result = await _service.GetAuctionByIdAsync(id);

        return FromResult(result);
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelAuction([FromRoute] long id)
    {
        ServiceResult result = await _service.CancelAuctionAsync(id);

        return FromResult(result);
    }
}
