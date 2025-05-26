using BidMasterOnline.Domain.Enums;

namespace Feedbacks.Service.API.DTO.Participant
{
    public class ParticipantSummaryComplaintDTO
    {
        public long Id { get; set; }

        public required string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public ComplaintStatus Status { get; set; }
    }
}
