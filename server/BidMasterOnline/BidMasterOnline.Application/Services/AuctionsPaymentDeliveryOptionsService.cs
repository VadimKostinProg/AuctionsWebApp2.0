using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Exceptions;
using BidMasterOnline.Application.RepositoryContracts;
using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Domain.Entities;

namespace BidMasterOnline.Application.Services
{
    public class AuctionsPaymentDeliveryOptionsService : IAuctionPaymentDelivaryService
    {
        private readonly IRepository _reporotiry;
        private readonly IAuthService _authService;
        private readonly INotificationsService _notificationsService;

        public AuctionsPaymentDeliveryOptionsService(IRepository reporotiry, IAuthService authService, INotificationsService notificationsService)
        {
            _reporotiry = reporotiry;
            _authService = authService;
            _notificationsService = notificationsService;
        }

        public async Task ConfirmDeliveyForAuctionAsync(ConfirmDeliveryDTO request)
        {
            var auction = await _reporotiry.GetByIdAsync<Auction>(request.AuctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.Status.Name == Enums.AuctionStatus.Canceled.ToString())
                throw new InvalidOperationException("Auction is canceled.");

            if (auction.Status.Name == Enums.AuctionStatus.Active.ToString())
                throw new InvalidOperationException("Active auction has not sell and delivery options.");

            var user = await _authService.GetAuthenticatedUserEntityAsync();

            if (auction.AuctionistId != user.Id)
                throw new ForbiddenException("You cannot confirm delivery for other user.");

            var paymentDeliveryOptions = auction.PaymentDeliveryOptions;

            if (!paymentDeliveryOptions.ArePaymentOptionsSet)
                throw new InvalidOperationException("Payment options has not been set yet.");

            if (!paymentDeliveryOptions.AreDeliveryOptionsSet)
                throw new InvalidOperationException("Delivery options has not been set yet.");

            if (paymentDeliveryOptions.IsDeliveryConfirmed)
                throw new InvalidOperationException("Delivery for this auction has been alreay confirmed.");

            if (string.IsNullOrEmpty(request.Waybill))
                throw new ArgumentException("Waybill is blank.");

            paymentDeliveryOptions.IsDeliveryConfirmed = true;
            paymentDeliveryOptions.DeliveryConfirmationDateTime = DateTime.Now;
            paymentDeliveryOptions.Waybill = request.Waybill;

            await _reporotiry.UpdateAsync(paymentDeliveryOptions);

            _notificationsService.SendMessageOfDeliveryConfirmationToWinner(auction, paymentDeliveryOptions.Winner);
        }

        public async Task ConfirmPaymentForAuctionAsync(Guid auctionId)
        {
            var auction = await _reporotiry.GetByIdAsync<Auction>(auctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.Status.Name == Enums.AuctionStatus.Canceled.ToString())
                throw new InvalidOperationException("Auction is canceled.");

            if (auction.Status.Name == Enums.AuctionStatus.Active.ToString())
                throw new InvalidOperationException("Active auction has not sell and delivery options.");

            var paymentDeliveryOptions = auction.PaymentDeliveryOptions;

            if (!paymentDeliveryOptions.ArePaymentOptionsSet)
                throw new InvalidOperationException("Payment options has not been set yet.");

            if (!paymentDeliveryOptions.AreDeliveryOptionsSet)
                throw new InvalidOperationException("Delivery options has not been set yet.");

            var user = await _authService.GetAuthenticatedUserEntityAsync();

            if (paymentDeliveryOptions.WinnerId != user.Id)
                throw new ForbiddenException("You cannot confirm payment for othe user.");

            if (paymentDeliveryOptions.IsPaymentConfirmed)
                throw new InvalidOperationException("Payment for this auction has been already confirmed.");

            paymentDeliveryOptions.IsPaymentConfirmed = true;
            paymentDeliveryOptions.PaymentConfirmationDateTime = DateTime.Now;

            await _reporotiry.UpdateAsync(paymentDeliveryOptions);

            _notificationsService.SendMessageOfPaymentConfirmationToAuctionist(auction);
        }

        public async Task<AuctionPaymentDeliveryOptionsDTO> GetPaymentDeliveryOptionsForAuctionByIdAsync(Guid auctionId)
        {
            var auction = await _reporotiry.GetByIdAsync<Auction>(auctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.Status.Name == Enums.AuctionStatus.Canceled.ToString())
                throw new InvalidOperationException("Auction is canceled.");

            if (auction.Status.Name == Enums.AuctionStatus.Active.ToString())
                throw new InvalidOperationException("Active auction has not sell and delivery options.");

            var paymentDeliveryOptions = auction.PaymentDeliveryOptions;

            return new AuctionPaymentDeliveryOptionsDTO
            {
                Id = paymentDeliveryOptions.Id,
                AuctionId = auctionId,
                WinnerId = paymentDeliveryOptions.WinnerId,
                Winner = paymentDeliveryOptions.WinnerId is not null ? 
                    paymentDeliveryOptions.Winner.Username : null,
                ArePaymentOptionsSet = paymentDeliveryOptions.ArePaymentOptionsSet,
                PaymentOptionsSetDateTime = 
                    paymentDeliveryOptions.PaymentOptionsSetDateTime is not null ?
                    paymentDeliveryOptions.PaymentOptionsSetDateTime.Value.ToString("yyyy-MM-dd HH:mm") 
                    : null,
                IBAN = paymentDeliveryOptions.IBAN,
                AreDeliveryOptionsSet = paymentDeliveryOptions.AreDeliveryOptionsSet,
                DeliveryOptionsSetDateTime =
                    paymentDeliveryOptions.DeliveryOptionsSetDateTime is not null ?
                    paymentDeliveryOptions.DeliveryOptionsSetDateTime.Value.ToString("yyyy-MM-dd HH:mm")
                    : null,
                Country = paymentDeliveryOptions.Country,
                City = paymentDeliveryOptions.City,
                ZipCode = paymentDeliveryOptions.ZipCode,
                IsPaymentConfirmed = paymentDeliveryOptions.IsPaymentConfirmed,
                PaymentConfirmationDateTime =
                    paymentDeliveryOptions.PaymentConfirmationDateTime is not null ?
                    paymentDeliveryOptions.PaymentConfirmationDateTime.Value.ToString("yyyy-MM-dd HH:mm")
                    : null,
                IsDeliveryConfirmed = paymentDeliveryOptions.IsDeliveryConfirmed,
                DeliveryConfirmationDateTime =
                    paymentDeliveryOptions.DeliveryConfirmationDateTime is not null ?
                    paymentDeliveryOptions.DeliveryConfirmationDateTime.Value.ToString("yyyy-MM-dd HH:mm")
                    : null,
                Waybill = paymentDeliveryOptions.Waybill
            };
        }

        public async Task SetDeliveryOptionsForAuctionAsync(SetDeliveryOptionsDTO request)
        {
            var auction = await _reporotiry.GetByIdAsync<Auction>(request.AuctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.Status.Name == Enums.AuctionStatus.Canceled.ToString())
                throw new InvalidOperationException("Auction is canceled.");

            if (auction.Status.Name == Enums.AuctionStatus.Active.ToString())
                throw new InvalidOperationException("Cannot set delivery options to active auction.");

            var paymentDeliveryOptions = auction.PaymentDeliveryOptions;

            if (paymentDeliveryOptions.WinnerId is null)
                throw new InvalidOperationException("Auction has no winners.");

            var user = await _authService.GetAuthenticatedUserEntityAsync();

            if (paymentDeliveryOptions.WinnerId != user.Id)
                throw new ForbiddenException("You cannot set delivery options for other user.");

            if (paymentDeliveryOptions.AreDeliveryOptionsSet)
                throw new InvalidOperationException("Delivery options for this auctions has been already set.");

            if (string.IsNullOrEmpty(request.Country))
                throw new ArgumentException("Country is blank.");

            if (string.IsNullOrEmpty(request.City))
                throw new ArgumentException("City is blank.");

            if (string.IsNullOrEmpty(request.ZipCode))
                throw new ArgumentException("ZIP code is blank.");

            paymentDeliveryOptions.AreDeliveryOptionsSet = true;
            paymentDeliveryOptions.DeliveryOptionsSetDateTime = DateTime.Now;
            paymentDeliveryOptions.Country = request.Country;
            paymentDeliveryOptions.City = request.City;
            paymentDeliveryOptions.ZipCode = request.ZipCode;

            await _reporotiry.UpdateAsync(paymentDeliveryOptions);

            if (paymentDeliveryOptions.ArePaymentOptionsSet)
            {
                _notificationsService.SendMessageOfApplyingPaymentToWinner(auction, paymentDeliveryOptions.Winner);
                _notificationsService.SendMessageOfApplyingDeliveryToAuctionist(auction);
            }
        }

        public async Task SetPaymentOptionsForAuctionAsync(SetPaymentOptionsDTO request)
        {
            var auction = await _reporotiry.GetByIdAsync<Auction>(request.AuctionId);

            if (auction is null)
                throw new KeyNotFoundException("Auction with such id does not exist.");

            if (auction.Status.Name == Enums.AuctionStatus.Canceled.ToString())
                throw new InvalidOperationException("Auction is canceled.");

            if (auction.Status.Name == Enums.AuctionStatus.Active.ToString())
                throw new InvalidOperationException("Cannot set payment options to active auction.");

            var user = await _authService.GetAuthenticatedUserEntityAsync();

            if (auction.AuctionistId != user.Id)
                throw new ForbiddenException("You cannot set payment options for other user.");

            var paymentDeliveryOptions = auction.PaymentDeliveryOptions;

            if (paymentDeliveryOptions.WinnerId is null)
                throw new InvalidOperationException("Auction has no winners.");

            if (paymentDeliveryOptions.ArePaymentOptionsSet)
                throw new InvalidOperationException("Payment options for this auction has been already set.");

            if (string.IsNullOrEmpty(request.IBAN))
                throw new ArgumentException("IBAN is blank.");

            paymentDeliveryOptions.ArePaymentOptionsSet = true;
            paymentDeliveryOptions.PaymentOptionsSetDateTime = DateTime.Now;
            paymentDeliveryOptions.IBAN = request.IBAN;

            await _reporotiry.UpdateAsync(paymentDeliveryOptions);

            if (paymentDeliveryOptions.AreDeliveryOptionsSet)
            {
                _notificationsService.SendMessageOfApplyingPaymentToWinner(auction, paymentDeliveryOptions.Winner);
                _notificationsService.SendMessageOfApplyingDeliveryToAuctionist(auction);
            }
        }
    }
}
