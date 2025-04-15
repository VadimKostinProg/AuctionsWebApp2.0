namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for technical support request. (RESPONSE)
    /// </summary>
    public class TechnicalSupportRequestDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string RequestText { get; set; }
        public string DateAndTime { get; set; }
        public bool IsHandled { get; set; }
    }
}
