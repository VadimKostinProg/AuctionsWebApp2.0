namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTP with user authentication information. (RESPONSE)
    /// </summary>
    public class AuthenticationDTO
    {
        public Guid UserId { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string Token { get; set; }
    }
}
