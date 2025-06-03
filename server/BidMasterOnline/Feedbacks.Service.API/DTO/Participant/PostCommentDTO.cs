using System.ComponentModel.DataAnnotations;

namespace Feedbacks.Service.API.DTO.Participant
{
    public class PostCommentDTO
    {
        public long AuctionId { get; set; }

        [Range(1, 10)]
        public int Score { get; set; }

        [MaxLength(5000)]
        public required string CommentText { get; set; }
    }
}
