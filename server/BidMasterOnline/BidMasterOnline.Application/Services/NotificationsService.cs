using BidMasterOnline.Application.ServiceContracts;
using BidMasterOnline.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace BidMasterOnline.Application.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        private readonly string _clientUrl;

        public NotificationsService(IEmailSender emailSender, IConfiguration configuration)
        {
            _emailSender = emailSender;
            _configuration = configuration;
            
            _clientUrl = _configuration["AllowedOrigins"]!;
        }

        public void SendMessageOfApplyingDeliveryToAuctionist(Auction auction)
        {
            var paymentDeliveryOptions = auction.PaymentDeliveryOptions;

            string title = "Apply the delivery of the auction lot.";

            string message = "Please apply the delivery of the lot in the next 3 days." +
                             "Here is information of the auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}<br>" +
                             $"Finish date and time: {auction.FinishDateTime}" +
                             "<br><br>Here is winners delivery information:<br>" +
                             $"County: {paymentDeliveryOptions.Country}<br>" +
                             $"City: {paymentDeliveryOptions.City}<br>" +
                             $"ZIP code: {paymentDeliveryOptions.ZipCode}<br>" +
                             $"Name: {paymentDeliveryOptions.Winner.FullName}<br>" +
                             $"Email: {paymentDeliveryOptions.Winner.Email}<br>" +
                             $"<br><br>To proove the delivery, please enter the waybill <a href=\"{_clientUrl}/apply-delivery?auctionId={auction.Id}\">here</a>.<br>" +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(auction.Auctionist.Email, auction.Auctionist.FullName, title, message);
        }

        public void SendMessageOfApplyingPaymentToWinner(Auction auction, User winner)
        {
            string title = "Bill for the auction lot.";

            string message = "You have received a bill for payment for the aucions lot." +
                             "Please apply the payment in the next 3 days. " +
                             "Here is information of the auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}" +
                             $"Finish date and time: {auction.FinishDateTime}" +
                             $"<br><br>To perform payment <a href=\"{_clientUrl}/apply-payment?auctionId={auction.Id}\">click here</a>." +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(winner.Email, winner.FullName, title, message);
        }

        public void SendMessageOfApprovalAuctionToAuctionist(Auction auction)
        {
            string title = "Your auction has been approved.";

            string message = "We are happy to inform you, that your auction has been successfully approved." +
                             "Now your auction is visible for other users and thay can set bids on your auction.<br>" +
                             "Here is information of the approved auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}" +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(auction.Auctionist.Email, auction.Auctionist.FullName, title, message);
        }

        public void SendMessageOfBlockingAccountToUser(User user, string blockingReason, int? days)
        {
            string title = "Your  account has been blocked.";

            string message = "We are informing you, that your account on the BidMasterOnline has been blocked." +
                             "Here is the blocking reason explained:<br>" +
                             blockingReason;

            if (days is not null)
                message = message + $"<br><br>Your account will be blocked for {days} days.";

            message = message + "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(user.Email, user.FullName, title, message);
        }

        public void SendMessageOfCancelingAuctionToAuctionist(Auction auction, string cancelingReason)
        {
            string title = "Your auction has been canceled.";

            string message = "We are informing you, that your auction has been canceled.<br>" +
                             "Here is information of the canceled auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}" +
                             $"<br><br>Here is the canceling reason explained:<br>" +
                             cancelingReason +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(auction.Auctionist.Email, auction.Auctionist.FullName, title, message);
        }

        public void SendMessageOfCancelingTheBidToWinner(Bid bid)
        {
            var auction = bid.Auction;

            string title = "Your account has been deleted.";

            string message = "We are informing you that your bid in the auciton, you have recently won, has been canceled, " +
                             "because you have not answered to the message to provide the delivery information." +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}" +
                             $"Finish date and time: {auction.FinishDateTime}" +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(bid.Bidder.Email, bid.Bidder.FullName, title, message);
        }

        public void SendMessageOfDeletingAccountToUser(User user)
        {
            string title = "Your account has been deleted.";

            string message = "We are informing you that your account has been deleted. " +
                             "Thank you for collaboration with BidMasterOnline!" +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(user.Email, user.FullName, title, message);
        }

        public void SendMessageOfDeliveryConfirmationToWinner(Auction auction, User winner)
        {
            var paymentDeliveryOptions = auction.PaymentDeliveryOptions;

            string title = "Your aucctions lot is on the way to you.";

            string message = "We are happy to inform you, that the lot of the auction, you have recently won, is on the way to you." +
                             "Auctionist has send the lot by the delivery service." +
                             $"<br><br>Here is the waybill: {paymentDeliveryOptions.Waybill}" +
                             "<br><br>Here is information of the auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}" +
                             $"Finish date and time: {auction.FinishDateTime}" +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(winner.Email, winner.FullName, title, message);
        }

        public void SendMessageOfDeliveryOptionsSetToWinner(Auction auction, User winner)
        {
            string title = "Your have won the auction!";

            string message = "We are happy to inform you, that you have won the auction!" +
                             "Here is information of the auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}<br>" +
                             $"Finish date and time: {auction.FinishDateTime}<br>" +
                             "<br>The next step is to set a delivery information to let auctionist send you the lot." +
                             "<br><br>Please set the delivery information in next 3 days, or your winning bid will be canceled. " +
                             $"To set a delivery information <a href=\"{_clientUrl}/set-delivery-options?auctionId={auction.Id}\">click here</a>." +
                             "<br><br>Later you will receive the bill to pay for the auctions lot." +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(winner.Email, winner.FullName, title, message);
        }

        public void SendMessageOfEmailConfirmToUser(User user)
        {
            string title = "Please confirm your email.";

            string message = "We are happy to inform you, that your account has been created successfully. " +
                             "But to have an opportunity to place bids at the auctions and create you own auctions " +
                             $"you need to confirm your email. " +
                             $"<br><br>To confirm your email <a href=\"{_clientUrl}/confirm-email?userId={user.Id}\">click here</a>." +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(user.Email, user.FullName, title, message);
        }

        public void SendMessageOfNoWinnersOfAuctionToAuctionist(Auction auction)
        {
            string title = "Your auction has been finished.";

            string message = "We are informing you that your auction has been finished. " +
                             "Unfortunately, there were no bids on your auction, so there is no winner of it." +
                             "Here is information of the finished auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}" +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(auction.Auctionist.Email, auction.Auctionist.FullName, title, message);
        }

        public void SendMessageOfPaymentConfirmationToAuctionist(Auction auction)
        {
            string title = "Your aucctions lot has been payed for.";

            string message = "We are happy to inform you, that the lot of your auction has been already payed by the winner." +
                             "Here is information of the auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}" +
                             $"Finish date and time: {auction.FinishDateTime}" +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(auction.Auctionist.Email, auction.Auctionist.FullName, title, message);
        }

        public void SendMessageOfPaymentOptionsSetToAuctionist(Auction auction)
        {
            string title = "Your aucction has been finished.";

            string message = "We are happy to inform you, that your auction has been finished." +
                             "Here is information of the auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}<br>" +
                             $"Finish date and time: {auction.FinishDateTime}<br>" +
                             "<br>The next step is to set a payment account to let winner of the auction pay for the lot." +
                             "<br><br>Please set the payment account in next 3 days." +
                             $"To set a payment account <a href=\"{_clientUrl}/set-payment-options?auctionId={auction.Id}\">click here</a>." +
                             "<br><br>Later you will receive the message with delivery information of the winner to send him the lot of the auciton." +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(auction.Auctionist.Email, auction.Auctionist.FullName, title, message);
        }

        public void SendMessageOfPublishingAuctionToAuctionst(Auction auction)
        {
            string title = "Your auction has been published for verification.";

            string message = "We are informing you that your auction has been published successfully. " +
                             "Now the auction is on the verification step. " +
                             "When auciton is approved, it will be visible for other users and they will be able to place bids at it." +
                             "Here is information of the published auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}" +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(auction.Auctionist.Email, auction.Auctionist.FullName, title, message);
        }

        public void SendMessageOfRecoveringAuctionToAuctionist(Auction auction)
        {
            string title = "Your auction has been recovered.";

            string message = "We are informing you, that your auction has been recovered.<br>" +
                             "Here is information of the recovered auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}" +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(auction.Auctionist.Email, auction.Auctionist.FullName, title, message);
        }

        public void SendMessageOfRejectionAuctionToAuctionist(Auction auction, string rejectionReason)
        {
            string title = "Your auction has been rejected.";

            string message = "We regret to inform you, that your auction has been rejected." +
                             "Here is information of the rejected auction:<br>" +
                             $"Auction Id: {auction.Id}<br>" +
                             $"Lot: {auction.Name}<br>" +
                             $"Description: {auction.LotDescription}<br>" +
                             $"Category: {auction.Category.Name}<br>" +
                             $"Start date and time: {auction.StartDateTime}" +
                             "<br><br>Heare is the rejection reason explained:<br>" +
                             rejectionReason +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(auction.Auctionist.Email, auction.Auctionist.FullName, title, message);
        }

        public void SendMessageOfSuccessfullConfirmingEmailToUser(User user)
        {
            string title = "Your email has been confirmed.";

            string message = "We are happy to inform you that your email has been confirmed. " +
                             "Now you have opportunity to place bids at the auction, write comments and " +
                             "set scores for them and create own aucitons. " +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(user.Email, user.FullName, title, message);
        }

        public void SendMessageOfUnblockingAccountToUser(User user)
        {
            string title = "Your account has been unblocked.";

            string message = "We are informing you that your account has been unblocked. " +
                             "Now you have opportunity to place bids at the auction, write comments and " +
                             "set scores for them and create own aucitons. " +
                             "<br><br>BidMasterOnline Technical Support Team.";

            _emailSender.SendEmail(user.Email, user.FullName, title, message);
        }
    }
}
