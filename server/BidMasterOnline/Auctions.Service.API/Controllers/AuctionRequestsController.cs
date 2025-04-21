using Auctions.Service.API.DTO;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers;

[Route("api/auctions/requests")]
public class AuctionRequestsController : BaseController
{
    private readonly IAuctionRequestsService _service;

    public AuctionRequestsController(IAuctionRequestsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserAuctionRequests()
    {
        ServiceResult<List<AuctionRequestSummaryDTO>> result = await _service.GetUserAuctionRequests();

        return FromResult(result);
    }



    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserAuctionRequestById([FromRoute] long id)
    {
        ServiceResult<AuctionRequestDTO> result = await _service.GetUserAuctionRequestById(id);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostAuctionRequest([FromBody] PostAuctionRequestDTO requestDTO)
    {
        ServiceResult<string> result = await _service.PostAuctionRequest(requestDTO);

        return FromResult(result);
    }

    [HttpPut("{id}/cancell")]
    public async Task<IActionResult> CancellAuctionRequest([FromRoute] long id)
    {
        ServiceResult<string> result = await _service.CancelAuctionRequestById(id);

        return FromResult(result);
    }
}
