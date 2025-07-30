using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.ServiceContracts;
using IdentityServer.Services.Contracts;

namespace IdentityServer.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsQueueProducer _queueProducer;

        public NotificationsService(INotificationsQueueProducer queueProducer)
        {
            _queueProducer = queueProducer;
        }

        public async Task SendEmailConfirmationMessageAsync(string email, string fullName, string code)
        {
            string title = "Email confirmation";

            string message = "Please, use the code below to confirm your email:<br><br>" +
                             $"<b>{code}</b>";

            EmailNotificationDTO notification = new()
            {
                RecipientEmail = email,
                RecipientName = fullName,
                Subject = title,
                BodyHtml = message
            };

            await _queueProducer.PushNotificationAsync(notification);
        }
    }
}
