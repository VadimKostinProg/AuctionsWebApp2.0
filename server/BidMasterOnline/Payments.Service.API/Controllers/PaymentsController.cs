using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payments.Service.API.DTO;
using Stripe;

namespace Payments.Service.API.Controllers
{
    [Route("api/participant/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Participant)]
    public class PaymentsController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IRepository repository,
            IUserAccessor userAccessor,
            ILogger<PaymentsController> logger)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _logger = logger;
        }

        [HttpPost("setup-intent")]
        public async Task<IActionResult> CreateSetupIntent()
        {
            ServiceResult<string> result = new();

            try
            {
                SetupIntentCreateOptions options = new()
                {
                    Usage = "off_session"
                };

                SetupIntentService service = new();
                SetupIntent setupIntent = await service.CreateAsync(options);

                result.Data = setupIntent.ClientSecret;

                return Ok(result);
            }
            catch (StripeException ex)
            {
                // TODO: log
                result.IsSuccessfull = false;
                result.Errors.Add("Failed to create setup intent.");
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Handled redirect from Stripe after completing SetupIntent on UI.
        /// UI redirects this endpoint.
        /// </summary>
        [HttpPost("setup-intent-confirm")]
        public async Task<IActionResult> SetupIntentComfirm([FromBody] SetupIntentConfirmRequest request)
        {
            ServiceResult result = new();

            try
            {
                long userId = _userAccessor.UserId;

                SetupIntentService service = new();
                SetupIntent setupIntent = await service.GetAsync(request.SetupIntentId);

                if (setupIntent.Status == "succeeded")
                {
                    string? stripeCustomerId = setupIntent.Customer?.Id;
                    string? paymentMethodId = setupIntent.PaymentMethodId;

                    if (string.IsNullOrEmpty(stripeCustomerId))
                    {
                        if (string.IsNullOrEmpty(paymentMethodId))
                        {
                            result.IsSuccessfull = false;
                            result.Errors.Add("An error occured while preforming payment.");
                            return StatusCode(500, result);
                        }

                        CustomerService customerService = new();
                        CustomerCreateOptions customerCreateOptions = new()
                        {
                            PaymentMethod = paymentMethodId,
                            Email = _userAccessor.Email,
                            Description = $"Customer for User ID: {userId}",
                        };
                        Customer newCustomer = await customerService.CreateAsync(customerCreateOptions);
                        stripeCustomerId = newCustomer.Id;
                    }

                    User user = await _repository.GetByIdAsync<User>(userId);

                    user.StripeCustomerId = stripeCustomerId;
                    user.PaymentMethodId = paymentMethodId;
                    user.IsPaymentMethodAttached = true;

                    _repository.Update(user);
                    await _repository.SaveChangesAsync();

                    result.Message = "Payment method attached successfully.";

                    return Ok(result);
                }
                else
                {
                    result.IsSuccessfull = false;
                    result.Errors.Add("An error occured while preforming payment.");
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                // TODO log
                result.IsSuccessfull = false;
                result.Errors.Add("An error occured while preforming payment.");
                return StatusCode(500, result);
            }
        }

        [HttpPost("charge-auction-win")]
        public async Task<IActionResult> ChargeAuctionWin([FromBody] AuctionPaymentRequest paymentRequest)
        {
            ServiceResult result = new();

            Auction auction = await _repository.GetByIdAsync<Auction>(paymentRequest.AuctionId,
                includeQuery: query => query.Include(e => e.Type)!);
            User payerUser = await _repository.GetByIdAsync<User>(_userAccessor.UserId);

            if (auction.Status != BidMasterOnline.Domain.Enums.AuctionStatus.Finished)
            {
                result.IsSuccessfull = false;
                result.Errors.Add("Auction is not finished yet.");
                return BadRequest(result);
            }

            if (!CheckPayerForAuction(auction, payerUser.Id))
            {
                result.IsSuccessfull = false;
                result.Errors.Add("Only winner of auction is allowed to perform payment.");
                return BadRequest(result);
            }

            string? stripeCustomerId = payerUser.StripeCustomerId;
            string? paymentMethodId = payerUser.PaymentMethodId;

            if (string.IsNullOrEmpty(stripeCustomerId))
            {
                result.IsSuccessfull = false;
                result.Errors.Add("Stripe Customer ID not found for this user. Please attach a payment method first.");
                return BadRequest(result);
            }

            if (string.IsNullOrEmpty(paymentMethodId))
            {
                result.IsSuccessfull = false;
                result.Errors.Add("Payment Method ID not found for this user. Please attach a payment method first.");
                return BadRequest(result);
            }

            if (auction.WinnerId == null || auction.WinnerId != payerUser.Id)
            {
                result.IsSuccessfull = false;
                result.Errors.Add("Only winner of auction allowed to proceed payment of it.");
                return BadRequest(result);
            }

            try
            {
                PaymentIntentService paymentIntentService = new();

                // Creating PaymentIntent
                PaymentIntentCreateOptions options = new()
                {
                    Amount = (long)(auction.FinishPrice!.Value * 100),
                    Currency = "usd",
                    Customer = stripeCustomerId,
                    PaymentMethod = paymentMethodId,
                    OffSession = true,
                    Confirm = true,
                    Metadata = new Dictionary<string, string>
                    {
                        { "userId", payerUser.Id.ToString() },
                        { "auctionId", paymentRequest.AuctionId.ToString() }
                    }
                };

                // Cpnfirmation of PaymentIntent
                PaymentIntent paymentIntent = await paymentIntentService.CreateAsync(options);

                if (paymentIntent.Status == "succeeded")
                {
                    auction.IsPaymentPerformed = true;
                    auction.PaymentPerformedTime = DateTime.UtcNow;

                    _repository.Update(auction);
                    await _repository.SaveChangesAsync();

                    result.Message = "Payment successful!";

                    return Ok(result);
                }
                else
                {
                    result.IsSuccessfull = false;
                    result.Errors.Add($"Payment failed or needs further action. Status: {paymentIntent.Status}");
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                //TODO: log
                result.IsSuccessfull = false;
                result.Errors.Add($"An error occured while performing payment.");
                return StatusCode(500, result);
            }
        }

        private bool CheckPayerForAuction(Auction auction, long payerId)
            => (auction.Type!.Name == AuctionTypes.DuchAuction && auction.AuctioneerId == payerId) ||
               (auction.Type!.Name != AuctionTypes.DuchAuction && auction.WinnerId == payerId);
    }
}
