using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Service.API.DTO;
using Payments.Service.API.ServiceContracts;

namespace Payments.Service.API.Controllers
{
    [Route("api/participant/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Participant)]
    public class PaymentsController : BaseController
    {
        private readonly IPaymentsService _service;

        public PaymentsController(IPaymentsService service)
        {
            _service = service;
        }

        [HttpPost("setup-intent")]
        public async Task<IActionResult> CreateSetupIntent()
        {
            ServiceResult<string> result = await _service.CreateSetupIntentAsync();

            return FromResult(result);
        }

        [HttpPost("setup-intent-confirm")]
        public async Task<IActionResult> SetupIntentComfirm([FromBody] SetupIntentConfirmRequest request)
        {
            ServiceResult result = await _service.ConfirmSetupIntentAsync(request);

            return FromResult(result);
        }

        [HttpPost("charge-auction-win")]
        public async Task<IActionResult> ChargeAuctionWin([FromBody] AuctionPaymentRequest paymentRequest)
        {
            ServiceResult result = await _service.ChargeAuctionWin(paymentRequest);

            return FromResult(result);
        }
    }
}
