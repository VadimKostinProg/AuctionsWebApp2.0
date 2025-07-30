namespace Feedbacks.Service.API.DTO.Participant
{
    public class SupportTicketDTO : SummarySupportTicketDTO
    {
        public required string Text { get; set; }

        public string? ModeratorComment { get; set; }
    }
}
