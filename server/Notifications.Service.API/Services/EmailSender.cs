using BidMasterOnline.Core.DTO;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;

namespace Notifications.Service.API.Services
{
    public class EmailSender
    {
        private readonly string _brevoApiKey;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailSender(IConfiguration configuration)
        {
            _brevoApiKey = configuration["BrevoSettings:ApiKey"]!;
            _senderEmail = configuration["BrevoSettings:SenderEmail"]!;
            _senderName = configuration["BrevoSettings:SenderName"]!;
        }

        public void SendEmailAsync(EmailNotificationDTO notification)
        {
            TransactionalEmailsApi apiInstance = new();
            SendSmtpEmailSender sender = new(_senderName, _senderEmail);

            SendSmtpEmailTo receiver = new(notification.RecipientEmail, notification.RecipientName);
            List<SendSmtpEmailTo> receiversList = [receiver];

            string subject = notification.Subject;
            string htmlContent = notification.BodyHtml;
            string? textContent = null;

            var sendSmtpEmail = new SendSmtpEmail(sender, receiversList, null, null, htmlContent, textContent, subject);
            apiInstance.SendTransacEmail(sendSmtpEmail);
        }
    }
}
