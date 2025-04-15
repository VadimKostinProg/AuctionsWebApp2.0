using BidMasterOnline.Application.Constants;
using BidMasterOnline.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BidMasterOnline.API.Controllers
{
    [Route("api/v{version:apiVersion}/checkout-session")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CheckoutAPIController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuctionsService _auctionService;

        public CheckoutAPIController(IConfiguration configuration, IAuctionsService auctionsService)
        {
            _configuration = configuration;
            _auctionService = auctionsService;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Customer)]
        public async Task<IActionResult> CreateSession([FromQuery] Guid auctionId)
        {
            var auctionDetails = await _auctionService.GetAuctionDetailsByIdAsync(auctionId);

            var token = Guid.NewGuid();

            var domain = _configuration["AllowedOrigins"];
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions{
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            Currency = "usd",
                            UnitAmount = (int)auctionDetails.CurrentBid * 100,
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = auctionDetails.Name,
                                Description = auctionDetails.LotDescription,
                                Images = auctionDetails.ImageUrls
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = domain + $"/payment-success?token={token}&auctionId={auctionDetails.Id}",
                CancelUrl = domain + "/payment-cancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new { SessionId = session.Id, Token = token });
        }
    }
}
