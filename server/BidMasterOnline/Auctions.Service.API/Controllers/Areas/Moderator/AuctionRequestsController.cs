using Auctions.Service.API.DTO.Moderator;
using Auctions.Service.API.ServiceContracts.Moderator;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers.Areas.Moderator;

[Route("api/moderator/auction-requests")]
[Authorize(Roles = UserRoles.Moderator)]
public class AuctionRequestsController : BaseController
{
    private readonly IAuctionRequestsService _service;

    public AuctionRequestsController(IAuctionRequestsService service)
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

    [HttpPut("approve")]
    public async Task<IActionResult> ApproveAuctionRequest([FromBody] ApproveAuctionRequestDTO requestDTO)
    {
        ServiceResult result = await _service.ApproveAuctionRequestAsync(requestDTO);

        return FromResult(result);
    }

    [HttpPut("decline")]
    public async Task<IActionResult> DeclineAuctionRequest([FromBody] DeclineAuctionRequestDTO requestDTO)
    {
        ServiceResult result = await _service.DeclineAuctionRequestAsync(requestDTO);

        return FromResult(result);
    }
}
