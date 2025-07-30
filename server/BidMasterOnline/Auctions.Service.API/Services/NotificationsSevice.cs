using Auctions.Service.API.ServiceContracts;
using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Models.Entities;

namespace Auctions.Service.API.Services
{
    public class NotificationsSevice : INotificationsService
    {
        private readonly INotificationsQueueProducer _notificationsQueue;
        private readonly IRepository _repository;

        private readonly string _participantUIClientUrl;

        public NotificationsSevice(INotificationsQueueProducer notificationsQueue,
            IRepository repository,
            IConfiguration configuration)
        {
            _notificationsQueue = notificationsQueue;
            _repository = repository;

            _participantUIClientUrl = configuration["Clients:ParticipantUI"]!;
        }

        public async Task SendMessageOfApprovalAuctionRequestToAuctioneer(AuctionRequest auctionRequest)
        {
            User recepient = await _repository.GetByIdAsync<User>(auctionRequest.RequestedByUserId);

            string title = "Your auction request has been approved!";

            string message = "We are happy to inform you, that your auction request has been successfully approved. " +
                             "Now your auction is visible for other users and thay can set bids on your auction.<br>" +
                             "Here is information of the approved auction:<br>" +
                             $"<b>Auction Id</b>: {auctionRequest.Id}<br>" +
                             $"<b>Lot</b>: {auctionRequest.LotTitle}<br>" +
                             $"<b>Description</b>: {auctionRequest.LotDescription}<br>" +
                             "<br><br>Best regards," +
                             "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = recepient.Email,
                RecipientName = recepient.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }

        public async Task SendMessageOfCancelingAuctionToAuctioneer(Auction auction)
        {
            User recepient = await _repository.GetByIdAsync<User>(auction.AuctioneerId);

            string title = "Your auction has been canceled.";

            string message = "We are informing you, that your auction has been canceled.<br>" +
                             "Here is information of the canceled auction:<br>" +
                             $"<b>Auction Id</b>: {auction.Id}<br>" +
                             $"<b>Lot</b>: {auction.LotTitle}<br>" +
                             $"<b>Description</b>: {auction.LotDescription}<br>" +
                             $"<br><br>Here is the canceling reason explained:<br>" +
                             auction.CancellationReason +
                             "<br><br>Best regards," +
                             "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = recepient.Email,
                RecipientName = recepient.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }

        public async Task SendMessageOfNoWinnersOfAuctionToAuctioneer(Auction auction)
        {
            User recepient = await _repository.GetByIdAsync<User>(auction.AuctioneerId);

            string title = "Your auction has been finished.";

            string message = "We are informing you that your auction has been finished. " +
                             "Unfortunately, there were no bids on your auction, so there is no winner of it." +
                             "Here is information of the finished auction:<br>" +
                             $"<b>Auction Id</b>: {auction.Id}<br>" +
                             $"<b>Lot</b>: {auction.LotTitle}<br>" +
                             $"<b>Description</b>: {auction.LotDescription}<br>" +
                             "<br><br>Best regards," +
                             "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = recepient.Email,
                RecipientName = recepient.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }

        public async Task SendMessageOfPublishingAuctionRequestToUserAsync(AuctionRequest auctionRequest)
        {
            User recepient = await _repository.GetByIdAsync<User>(auctionRequest.RequestedByUserId);

            string title = "Your auction request has been published for verification.";

            string message = "We are informing you that your auction request has been published successfully. " +
                             "Now the auction request is on the verification step. " +
                             "When it is approved, new auction will be created and visible for other users, so they will be able to place bids at it." +
                             "Here is information of the published auction request:<br>" +
                             $"<b>Auction request Id</b>: {auctionRequest.Id}<br>" +
                             $"<b>Lot</b>: {auctionRequest.LotTitle}<br>" +
                             $"<b>Description</b>: {auctionRequest.LotDescription}<br>" +
                             "<br><br>Best regards," +
                             "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = recepient.Email,
                RecipientName = recepient.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }

        public async Task SendMessageOfRecoveringAuctionToAuctioneer(Auction auction)
        {
            User recepient = await _repository.GetByIdAsync<User>(auction.AuctioneerId);

            string title = "Your auction has been recovered.";

            string message = "We are informing you, that your auction has been recovered.<br>" +
                             "Here is information of the recovered auction:<br>" +
                             $"<b>Auction Id</b>: {auction.Id}<br>" +
                             $"<b>Lot</b>: {auction.LotTitle}<br>" +
                             $"<b>Description</b>: {auction.LotDescription}<br>" +
                             "<br><br>Best regards," +
                             "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = recepient.Email,
                RecipientName = recepient.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }

        public async Task SendMessageOfDecliningAuctionRequestToUser(AuctionRequest auctionRequest)
        {
            User recepient = await _repository.GetByIdAsync<User>(auctionRequest.RequestedByUserId);

            string title = "Your auction request has been declined.";

            string message = "We are informing you that, unfortunately, your auction request has been declined. " +
                             "Here is information of the auction request:<br>" +
                             $"<b>Auction request Id</b>: {auctionRequest.Id}<br>" +
                             $"<b>Lot</b>: {auctionRequest.LotTitle}<br>" +
                             $"<b>Description</b>: {auctionRequest.LotDescription}<br>" +
                             $"<br><br>Here is the declining reason explained:<br>" +
                             auctionRequest.ReasonDeclined +
                             "<br><br>Best regards," +
                             "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = recepient.Email,
                RecipientName = recepient.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }

        public async Task SendMessageOfFinishingAuctionToSeller(Auction auction)
        {
            bool isWinner = auction.Type!.Name == AuctionTypes.DutchAuction;

            long recipientId = isWinner 
                ? auction.WinnerId!.Value
                : auction.AuctioneerId;

            User recepient = await _repository.GetByIdAsync<User>(recipientId);

            string performDeliveryPageLink = $"{_participantUIClientUrl}/auctions/{auction.Id}/perform-delivery";

            string title = isWinner
                ? "You have won the auction!"
                : "Your auction has finished!";

            string message = isWinner 
                ? "Congradulations! You have just won the auction!<br>"
                : "Congradulations! Your auction has just finished! And you have a winner of auction!<br>";

            message += "Here is information of the auction:<br>" +
                       $"<b>Auction Id</b>: {auction.Id}<br>" +
                       $"<b>Lot</b>: {auction.LotTitle}<br>" +
                       $"<b>Description</b>: {auction.LotDescription}<br>" +
                       $"<b>Finish price</b>: ${auction.FinishPrice}<br>" +
                       $"<br><br>Please, go to the <a href=\"{performDeliveryPageLink}\">delivery perorming page</a> and perform delivery of the auction lot." +
                       "We hope you will manage to perform delivery within 3 days not to make any inconviniance for buyer." +
                       "<br><br>Best regards," +
                       "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = recepient.Email,
                RecipientName = recepient.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }

        public async Task SendMessageOfFinishingAuctionToBuyer(Auction auction)
        {
            bool isWinner = auction.Type!.Name != AuctionTypes.DutchAuction;

            long recipientId = isWinner
                ? auction.WinnerId!.Value
                : auction.AuctioneerId;

            User recepient = await _repository.GetByIdAsync<User>(recipientId);

            string checkoutPageLink = $"{_participantUIClientUrl}/auctions/{auction.Id}/checkout";

            string title = isWinner
                ? "You have won the auction!"
                : "Your auction has finished!";

            string message = isWinner
                ? "Congradulations! You have just won the auction!<br>"
                : "Congradulations! Your auction has just finished! And you have a winner of auction!<br>";

            message += "Here is information of the auction:<br>" +
                       $"<b>Auction Id</b>: {auction.Id}<br>" +
                       $"<b>Lot</b>: {auction.LotTitle}<br>" +
                       $"<b>Description</b>: {auction.LotDescription}<br>" +
                       $"<b>Finish price</b>: ${auction.FinishPrice}<br>" +
                       $"<br><br>Please, go to the <a href=\"{checkoutPageLink}\">checkout page</a> and peform payment for auction lot." +
                       "We hope you will manage to perform payment within 3 days not to make any inconviniance for seller." +
                       "<br><br>Best regards," +
                       "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = recepient.Email,
                RecipientName = recepient.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }

        public async Task SendMessageOfPerformingDeliveryToBuyer(Auction auction)
        {
            bool isWinner = auction.Type!.Name != AuctionTypes.DutchAuction;

            long recipientId = isWinner
                ? auction.WinnerId!.Value
                : auction.AuctioneerId;

            User recepient = await _repository.GetByIdAsync<User>(recipientId);

            string title = "Seller has performed delivery for your auction!";

            string message = "Seller has just performed delivery for your auction!" +
                            "Here is information of the auction:<br>" +
                            $"<b>Auction Id</b>: {auction.Id}<br>" +
                            $"<b>Lot</b>: {auction.LotTitle}<br>" +
                            $"<b>Description</b>: {auction.LotDescription}<br>" +
                            $"<b>Finish price</b>: ${auction.FinishPrice}<br>" +
                            $"<br><br>Please, use this waybill to track the delivery:<br>" +
                            auction.DeliveryWaybill +
                            "<br><br>Best regards," +
                            "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = recepient.Email,
                RecipientName = recepient.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }

        public async Task SendMessageOfStartingAuctionToAuctioneer(Auction auction)
        {
            User recepient = await _repository.GetByIdAsync<User>(auction.AuctioneerId);

            string title = "Your auction has started!";

            string message = "We are informing you, that your auction has been started.<br>" +
                             "Here is information of the recovered auction:<br>" +
                             $"<b>Auction Id</b>: {auction.Id}<br>" +
                             $"<b>Lot</b>: {auction.LotTitle}<br>" +
                             $"<b>Description</b>: {auction.LotDescription}<br>" +
                             "<br><br>Best regards," +
                             "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = recepient.Email,
                RecipientName = recepient.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }
    }
}
