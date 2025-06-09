using BidMasterOnline.Core.Constants;
using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Models.Entities;
using Payments.Service.API.ServiceContracts;

namespace Payments.Service.API.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsQueueProducer _notificationsQueue;
        private readonly IRepository _repository;

        public NotificationsService(INotificationsQueueProducer notificationsQueue,
            IRepository repository)
        {
            _notificationsQueue = notificationsQueue;
            _repository = repository;
        }

        public async Task SendMessageOfPerformingPaymentToSeller(Auction auction)
        {
            bool isWinner = auction.Type!.Name == AuctionTypes.DutchAuction;
            long recipientId = isWinner
                ? auction.WinnerId!.Value
                : auction.AuctioneerId;

            User recepient = await _repository.GetByIdAsync<User>(recipientId);

            string title = "Buyer has performed payment for your auction!";

            string message = "Buyer has just performed payment for your auction!" +
                            "Here is information of the auction:<br>" +
                            $"<b>Auction Id</b>: {auction.Id}<br>" +
                            $"<b>Lot</b>: {auction.LotTitle}<br>" +
                            $"<b>Description</b>: {auction.LotDescription}<br>" +
                            $"<b>Finish price</b>: ${auction.FinishPrice}<br>" +
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
