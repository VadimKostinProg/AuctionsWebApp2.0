namespace Feedbacks.Service.API.DTO.Participant
{
    public class PostUserFeedbackDTO
    {
        public long ToUserId { get; set; }

        public int Score { get; set; }

        public string? Comment { get; set; }
    }
}
