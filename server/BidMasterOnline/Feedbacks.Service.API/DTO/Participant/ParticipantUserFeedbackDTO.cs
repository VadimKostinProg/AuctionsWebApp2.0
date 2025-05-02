namespace Feedbacks.Service.API.DTO.Participant
{
    public class ParticipantUserFeedbackDTO
    {
        public long Id { get; set; }

        public long FromUserId { get; set; }

        public int Score { get; set; }

        public string? Comment { get; set; }

        public string FromUsername { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
