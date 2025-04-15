namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for user profile. (RESPONSE)
    /// </summary>
    public class ProfileDTO : UserDTO
    {
        public int TotalAuctions { get; set; }
        public int TotalWins { get; set; }
    }
}
