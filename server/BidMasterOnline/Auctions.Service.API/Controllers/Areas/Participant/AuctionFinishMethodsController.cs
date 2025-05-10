using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers.Areas.Participant
{
    [Route("api/participant/auction-finish-methods")]
    [Authorize(Roles = UserRoles.Participant)]
    public class AuctionFinishMethodsController : BaseController
    {
        private readonly IParticipantAuctionFinishMethodsService _service;

        public AuctionFinishMethodsController(IParticipantAuctionFinishMethodsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuctionCaregories()
        {
            ServiceResult<List<AuctionFinishMethodDTO>> result = await _service
                .GetAuctionFinishMethodsAsync();

            return FromResult(result);
        }
    }
}
