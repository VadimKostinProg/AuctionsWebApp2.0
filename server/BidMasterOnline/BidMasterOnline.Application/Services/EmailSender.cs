using BidMasterOnline.Application.ServiceContracts;
using Microsoft.Extensions.Configuration;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;

namespace BidMasterOnline.Application.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        private readonly string SenderEmail;
        private readonly string SenderName;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;

            this.SenderEmail = _configuration["BrevoSettings:SenderEmail"]!;
            this.SenderName = _configuration["BrevoSettings:SenderName"]!;
        }

        public void SendEmail(string receiverEmail, string receiverName, string subject, string message)
        {
            var apiInstance = new TransactionalEmailsApi();
            var sender = new SendSmtpEmailSender(SenderName, SenderEmail);

            var receiver = new SendSmtpEmailTo(receiverEmail, receiverName);
            var receiversList = new List<SendSmtpEmailTo> { receiver };


            string htmlContent = message;
            string textContent = null;

            var sendSmtpEmail = new SendSmtpEmail(sender, receiversList, null, null, htmlContent, textContent, subject);
            apiInstance.SendTransacEmail(sendSmtpEmail);
        }
    }
}
