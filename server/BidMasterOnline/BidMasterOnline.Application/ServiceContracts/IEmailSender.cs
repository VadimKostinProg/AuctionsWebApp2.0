namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for sending notifications.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends the notifications to specified user with specified title and message.
        /// </summary>
        /// <param name="email">Email of recipient.</param>
        /// <param name="subject">Subject of the notification.</param>
        /// <param name="message">Message of the notification.</param>
        void SendEmail(string receiverEmail, string receiverName, string subject, string message);
    }
}
