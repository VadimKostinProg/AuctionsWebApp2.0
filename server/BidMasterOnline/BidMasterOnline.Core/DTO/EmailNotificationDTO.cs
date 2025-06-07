namespace BidMasterOnline.Core.DTO
{
    public class EmailNotificationDTO
    {
        public required string RecipientEmail { get; set; }
        public required string RecipientName { get; set; }
        public required string Subject { get; set; }
        public required string BodyHtml { get; set; }
    }
}
