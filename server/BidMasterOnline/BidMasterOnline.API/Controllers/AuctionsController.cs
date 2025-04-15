using BidMasterOnline.Application.Constants;
using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidMasterOnline.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionsService _auctionsService;
        private readonly IAuctionVerificationService _auctionVerificationService;
        private readonly IAuctionPaymentDelivaryService _auctionSellDelivaryService;
        private readonly IBidsService _bidsService;

        public AuctionsController(IAuctionsService auctionsService,
            IAuctionVerificationService auctionVerificationService,
            IAuctionPaymentDelivaryService auctionSellDelivaryService,
            IBidsService bidsService)
        {
            _auctionsService = auctionsService;
            _auctionVerificationService = auctionVerificationService;
            _auctionSellDelivaryService = auctionSellDelivaryService;
            _bidsService = bidsService;
        }

        [HttpGet("list")]
        [AllowAnonymous]
        public async Task<ActionResult<ListModel<AuctionDTO>>> GetAuctionsList(
            [FromQuery] AuctionSpecificationsDTO specifications)
        {
            return Ok(await _auctionsService.GetAuctionsListAsync(specifications));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById([FromRoute] Guid id)
        {
            return Ok(await _auctionsService.GetAuctionByIdAsync(id));
        }

        [HttpGet("{id}/details")]
        [AllowAnonymous]
        public async Task<ActionResult<AuctionDetailsDTO>> GetAuctionDetailsById([FromRoute] Guid id)
        {
            return Ok(await _auctionsService.GetAuctionDetailsByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult> PublishAuction([FromForm] PublishAuctionDTO auction)
        {
            await _auctionsService.PublishAuctionAsync(auction);

            return Ok(new { Message = "Auction has been successfully published for verification." });
        }

        [HttpPost("scores")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult> SetScoreForAuction([FromBody] SetAuctionScoreDTO request)
        {
            await _auctionsService.SetAuctionScoreAsync(request);

            return Ok(new { Message = "Your score for auction has been successfully set." });
        }

        [HttpPut("cancel")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult> CancelAuction([FromBody] CancelAuctionDTO request)
        {
            await _auctionsService.CancelAuctionAsync(request);

            return Ok(new { Message = "Auction has been canceled successfully." });
        }

        [HttpPut("own/{id}/cancel")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult> CancelOwnAuction([FromRoute] Guid id)
        {
            await _auctionsService.CancelOwnAuctionAsync(id);

            return Ok(new { Message = "Auction has been canceled successfully." });
        }

        [HttpPut("{id}/recover")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult> RecoverAuction([FromRoute] Guid id)
        {
            await _auctionsService.RecoverAuctionAsync(id);

            return Ok(new { Message = "Auction has been recovered successfully." });
        }

        [HttpGet("{id}/bids")]
        [AllowAnonymous]
        public async Task<ActionResult<ListModel<BidDTO>>> GetBidsForAuction([FromRoute] Guid id,
            [FromQuery] SpecificationsDTO specifications)
        {
            return Ok(await _bidsService.GetBidsListForAuctionAsync(id, specifications));
        }

        [HttpPost("bids")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult> SetNewBid([FromBody] SetBidDTO bid)
        {
            await _bidsService.SetBidAsync(bid);

            return Ok(new { Message = "New bid has been set successfully." });
        }

        [HttpDelete("{id}/bids")]
        [Authorize(Roles = UserRoles.TechnicalSupportSpecialist)]
        public async Task<ActionResult> CancelLastBidOfAuction([FromRoute] Guid id)
        {
            await _auctionsService.SetNextWinnerOfAuctionAsync(id);

            return Ok(new { Message = "The last bid has been canceled successfully."});
        }

        [HttpGet("not-confirmed")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult<IEnumerable<AuctionDTO>>> GetFinishedAuctionWithNotConfirmedOptions(
            [FromQuery] Application.Enums.AuctionParticipant participant)
        {
            return Ok(await _auctionsService.GetFinishedAuctionsWithNotConfirmedOptionsAsync(participant));
        }

        [HttpGet("not-approved/list")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<ListModel<AuctionDTO>>> GetNotApprovedAuctionsList(
            [FromQuery] AuctionSpecificationsDTO specifications)
        {
            return Ok(await _auctionVerificationService.GetNotApprovedAuctionsListAsync(specifications));
        }

        [HttpGet("not-approved/{id}/details")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<AuctionDetailsDTO>> GetNotApprovedAuctionDetails([FromRoute] Guid id)
        {
            return Ok(await _auctionVerificationService.GetNotApprovedAuctionDetailsByIdAsync(id));
        }

        [HttpPut("not-approved/{id}/approve")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> ApproveAuction([FromRoute] Guid id)
        {
            await _auctionVerificationService.ApproveAuctionAsync(id);

            return Ok(new { Message = "Auction has been approved successfully." });
        }

        [HttpPut("not-approved/reject")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> RejectAuction([FromBody] RejectAuctionDTO request)
        {
            await _auctionVerificationService.RejectAuctionAsync(request);

            return Ok(new { Message = "Auction has been rejected successfully." });
        }

        [HttpGet("{id}/payment-delivery-options")]
        [Authorize(Roles = $"{UserRoles.Customer}, {UserRoles.TechnicalSupportSpecialist}")]
        public async Task<ActionResult<AuctionPaymentDeliveryOptionsDTO>> GetPaymentDeliveryOptions([FromRoute] Guid id)
        {
            return Ok(await _auctionSellDelivaryService.GetPaymentDeliveryOptionsForAuctionByIdAsync(id));
        }

        [HttpPost("payment-options")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult> SetPaymentOptions([FromBody] SetPaymentOptionsDTO request)
        {
            await _auctionSellDelivaryService.SetPaymentOptionsForAuctionAsync(request);

            return Ok(new { Message = "Payment options for the auction has been successfully set." });
        }

        [HttpPost("delivery-options")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult> SetDeliveryOptions([FromBody] SetDeliveryOptionsDTO request)
        {
            await _auctionSellDelivaryService.SetDeliveryOptionsForAuctionAsync(request);

            return Ok(new { Message = "Delivery options for the auction has been successfully set." });
        }

        [HttpPut("payment-options")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult> ConfirmPayment([FromQuery] Guid auctionId)
        {
            await _auctionSellDelivaryService.ConfirmPaymentForAuctionAsync(auctionId);

            return Ok(new { Message = "Payment for the auction has been successfully confirmed." });
        }

        [HttpPut("delivery-options")]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<ActionResult> ConfirmDelivery([FromBody] ConfirmDeliveryDTO request)
        {
            await _auctionSellDelivaryService.ConfirmDeliveyForAuctionAsync(request);

            return Ok(new { Message = "Delivery for the auction has been successfully confirmed." });
        }
    }
}