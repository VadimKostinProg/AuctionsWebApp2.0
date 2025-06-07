using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Models.Entities;
using Users.Service.API.ServiceContracts;

namespace Users.Service.API.Services
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

        public async Task SendMessageOfBlockingUserAsync(User user)
        {
            string title = "Your account has been blocked.";

            string message = "We are informing you that your account on the BidMasterOnline has been blocked." +
                             "<br>Here is the reason of blocking you account explained:<br>" +
                             user.BlockingReason +
                             "<br><br>Best regards," +
                             "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = user.Email,
                RecipientName = user.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }

        public async Task SendMessageOfUnblockingUserAsync(User user)
        {
            string title = "Your account has been unblocked.";

            string message = "We are informing you that your account on the BidMasterOnline has been unblocked." +
                             "<br><br>Best regards," +
                             "<br>BidMasterOnline Technical Support Team.";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = user.Email,
                RecipientName = user.FullName,
                Subject = title,
                BodyHtml = message
            };

            await _notificationsQueue.PushNotificationAsync(notification);
        }
    }
}
