namespace Feedbacks.Service.API.DTO.Participant
{
    public class ParticipantSummarySupportTicketDTO
    {
        public long Id { get; set; }

        public required string Title { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
