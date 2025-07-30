namespace Feedbacks.Service.API.DTO.Participant
{
    public class AuctionCommentDTO
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long AuctionId { get; set; }

        public int Score { get; set; }

        public required string CommentText { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Username { get; set; } = string.Empty;
    }
}
