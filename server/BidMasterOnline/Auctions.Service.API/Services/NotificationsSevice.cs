using Auctions.Service.API.ServiceContracts;
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

        public NotificationsSevice(INotificationsQueueProducer notificationsQueue,
            IRepository repository)
        {
            _notificationsQueue = notificationsQueue;
            _repository = repository;
        }

        public async Task SendMessageOfApprovalAuctionRequestToAuctioneer(AuctionRequest auctionRequest)
        {
            User recepient = await _repository.GetByIdAsync<User>(auctionRequest.RequestedByUserId);

            string title = "Your auction request has been approved!";

            string message = "We are happy to inform you, that your auction request has been successfully approved." +
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

        // TODO: move to Users.Service.API
        public async Task SendMessageOfDeletingAccountToUser(User recepient)
        {
            string title = "Your account has been deleted.";

            string message = "We are informing you that your account has been deleted. " +
                             "Thank you for collaboration with BidMasterOnline!" +
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
    }
}
