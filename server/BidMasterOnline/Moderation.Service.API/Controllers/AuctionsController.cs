using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using Moderation.Service.API.DTO;
using Moderation.Service.API.ServiceContracts;

namespace Moderation.Service.API.Controllers
{
    [Route("api/moderation/[controller]")]
    [ApiController]
    public class AuctionsController : BaseController
    {
        private readonly IAuctionsService _service;

        [HttpPost("requests/approve")]
        public async Task<IActionResult> ApproveAuctionRequest([FromBody] ApproveAuctionRequestDTO requestDTO)
        {
            ServiceResult result = await _service.ApproveAuctionRequestAsync(requestDTO);

            return FromResult(result);
        }

        [HttpPost("requests/decline")]
        public async Task<IActionResult> DeclineAuctionRequest([FromBody] DeclineAuctionRequestDTO requestDTO)
        {
            ServiceResult result = await _service.DeclineAuctionRequestAsync(requestDTO);

            return FromResult(result);
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelAuction([FromBody] CancelAuctionDTO requestDTO)
        {
            ServiceResult result = await _service.CancelAuctionAsync(requestDTO);

            return FromResult(result);
        }

        [HttpPost("recover")]
        public async Task<IActionResult> RecoverAuction([FromBody] RecoverAuctionDTO requestDTO)
        {
            ServiceResult result = await _service.RecoverAuctionAsync(requestDTO);

            return FromResult(result);
        }
    }
}
