using Auctions.Service.API.DTO.Participant;
using Auctions.Service.API.ServiceContracts.Participant;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctions.Service.API.Controllers.Areas.Participant
{
    [Route("api/participant/auction-categories")]
    [Authorize(Roles = UserRoles.Participant)]
    public class AuctionCategoriesController : BaseController
    {
        private readonly IAuctionCategoriesService _service;

        public AuctionCategoriesController(IAuctionCategoriesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuctionCaregories()
        {
            ServiceResult<List<AuctionCategoryDTO>> result = await _service.GetAuctionCategoriesAsync();

            return FromResult(result);
        }
    }
}
