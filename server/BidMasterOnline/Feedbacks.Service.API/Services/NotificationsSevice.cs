using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.RepositoryContracts;
using BidMasterOnline.Core.ServiceContracts;
using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models.Entities;
using Feedbacks.Service.API.ServiceContracts;

namespace Feedbacks.Service.API.Services
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

        public async Task SendMessageOfProcessingComplaintAsync(Complaint complaint)
        {
            User recepient = await _repository.GetByIdAsync<User>(complaint.AccusingUserId);

            string title = "Your complaint has been published.";

            string message = "We are informing you, that your complaint has been successfully saved, " +
                             "so our moderators will carefully invastigate the issue and notify you on every updates." +
                             "Here are details of the complaint:<br>" +
                             $"<b>Complaint Id</b>: {complaint.Id}<br>" +
                             $"<b>Complaint type</b>: {GetStringOfComplaintType(complaint.Type)}<br>" +
                             $"<b>Complaint text</b>: <br>" +
                             complaint.ComplaintText +
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

        public async Task SendMessageOfProcessingSupportTicketAsync(SupportTicket supportTicket)
        {
            User recepient = await _repository.GetByIdAsync<User>(supportTicket.UserId);

            string title = "Your support ticket has been published.";

            string message = "We are informing you, that your support ticket has been successfully saved, " +
                             "so our moderators will carefully check your issue and notify you on every updates." +
                             "Here are details of the support ticket:<br>" +
                             $"<b>Support ticket Id</b>: {supportTicket.Id}<br>" +
                             $"<b>Title</b>: {supportTicket.Title}<br>" +
                             $"<b>Text</b>: <br>" +
                             supportTicket.Text +
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

        public async Task SendMessageOfCompletingComplaintAsync(Complaint complaint)
        {
            User recepient = await _repository.GetByIdAsync<User>(complaint.AccusingUserId);

            string title = "Your complaint has been resolved.";

            string message = "We are informing you, that your complaint has been resolved. " +
                             "Here are details of the complaint:<br>" +
                             $"<b>Complaint Id</b>: {complaint.Id}<br>" +
                             $"<b>Complaint type</b>: {GetStringOfComplaintType(complaint.Type)}<br>" +
                             $"<b>Complaint text</b>: <br>" +
                             complaint.ComplaintText +
                             "<br><br>Here is a moderator's conclusion:<br>" +
                             complaint.ModeratorConclusion +
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

        public async Task SendMessageOfCompletingSupportTicketAsync(SupportTicket supportTicket)
        {
            User recepient = await _repository.GetByIdAsync<User>(supportTicket.UserId);

            string title = "Your support issue has been resolved.";

            string message = "We are informing you, that your support issue has been resolved. " +
                             "Here are details of the support ticket:<br>" +
                             $"<b>Support ticket Id</b>: {supportTicket.Id}<br>" +
                             $"<b>Title</b>: {supportTicket.Title}<br>" +
                             $"<b>Text</b>: <br>" +
                             supportTicket.Text +
                             "<br><br>Here is a moderator's conmment:<br>" +
                             supportTicket.ModeratorComment +
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

        private string GetStringOfComplaintType(ComplaintType type)
            => type switch
            {
                ComplaintType.ComplaintOnAuctionComment => "Complaint on auction comment",
                ComplaintType.ComplaintOnAuctionContent => "Complaint on auction content",
                ComplaintType.ComplaintOnUserBehaviour => "Complaint on user behaviour",
                _ => "Complaint on user feedback",
            };
    }
}
