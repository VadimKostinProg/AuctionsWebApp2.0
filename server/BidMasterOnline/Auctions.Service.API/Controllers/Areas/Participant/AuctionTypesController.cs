using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers.Areas.Participant
{
    [Route("api/participant/auction-types")]
    [Authorize(Roles = UserRoles.Participant)]
    public class AuctionTypesController : BaseController
    {
        private readonly IAuctionTypesService _service;

        public AuctionTypesController(IAuctionTypesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuctionCaregories()
        {
            ServiceResult<List<AuctionTypeDTO>> result = await _service.GetAuctionTypesAsync();

            return FromResult(result);
        }
    }
}
