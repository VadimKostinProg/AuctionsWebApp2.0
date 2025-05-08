using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers.Areas.Participant;

[Route("api/participant/auction-requests")]
[Authorize(Roles = UserRoles.Participant)]
public class AuctionRequestsController : BaseController
{
    private readonly IAuctionRequestsService _service;

    public AuctionRequestsController(IAuctionRequestsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserAuctionRequests([FromQuery] PaginationRequestDTO pagination)
    {
        ServiceResult<PaginatedList<AuctionRequestSummaryDTO>> result = await _service
            .GetUserAuctionRequestsAsync(pagination);

        return FromResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuctionRequestById([FromRoute] long id)
    {
        ServiceResult<AuctionRequestDTO> result = await _service.GetAuctionRequestByIdAsync(id);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostAuctionRequest([FromForm] PostAuctionRequestDTO requestDTO)
    {
        ServiceResult result = await _service.PostAuctionRequestAsync(requestDTO);

        return FromResult(result);
    }

    [HttpPut("{id}/cancell")]
    public async Task<IActionResult> CancellAuctionRequest([FromRoute] long id)
    {
        ServiceResult result = await _service.CancelAuctionRequestByIdAsync(id);

        return FromResult(result);
    }
}
