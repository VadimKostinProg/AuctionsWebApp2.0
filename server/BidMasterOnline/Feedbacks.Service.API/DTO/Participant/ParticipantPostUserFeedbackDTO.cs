namespace Feedbacks.Service.API.DTO.Participant
{
    public class ParticipantPostUserFeedbackDTO
    {
        public long ToUserId { get; set; }

        public int Score { get; set; }

        public string? Comment { get; set; }
    }
}
