namespace BidMasterOnline.Core.DTO
{
    public class UserSummaryDTO
    {
        public long UserId { get; set; }

        public required string Username { get; set; }

        public required string Email { get; set; }
    }
}
