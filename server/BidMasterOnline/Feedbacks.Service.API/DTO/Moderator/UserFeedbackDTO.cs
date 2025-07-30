using BidMasterOnline.Core.DTO;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class UserFeedbackDTO : BaseDTO
    {
        public long FromUserId { get; set; }

        public long ToUserId { get; set; }

        public int Score { get; set; }

        public string? Comment { get; set; }

        public string FromUsername { get; set; } = string.Empty;

        public string ToUsername { get; set; } = string.Empty;
    }
}
