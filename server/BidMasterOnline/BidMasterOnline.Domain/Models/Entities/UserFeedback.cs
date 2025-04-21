namespace BidMasterOnline.Domain.Models.Entities
{
    public class UserFeedback : EntityBase
    {
        public long FromUserId { get; set; }

        public long ToUserId { get; set; }

        public int Score { get; set; }

        public string? Comment { get; set; }
    }
}
