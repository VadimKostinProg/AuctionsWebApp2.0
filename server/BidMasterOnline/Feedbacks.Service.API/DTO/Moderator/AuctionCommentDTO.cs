using BidMasterOnline.Core.DTO;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class AuctionCommentDTO : BaseDTO
    {
        public long UserId { get; set; }

        public long AuctionId { get; set; }

        public int Score { get; set; }

        public required string CommentText { get; set; }

        public string Username { get; set; } = string.Empty;
    }
}
