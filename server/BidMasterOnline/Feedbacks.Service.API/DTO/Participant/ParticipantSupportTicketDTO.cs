namespace Feedbacks.Service.API.DTO.Participant
{
    public class ParticipantSupportTicketDTO : ParticipantSummarySupportTicketDTO
    {
        public required string Text { get; set; }

        public string? ModeratorComment { get; set; }
    }
}
