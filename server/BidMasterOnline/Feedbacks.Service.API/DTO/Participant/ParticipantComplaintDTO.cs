using BidMasterOnline.Domain.Enums;

namespace Feedbacks.Service.API.DTO.Participant
{
    public class ParticipantComplaintDTO : ParticipantSummaryComplaintDTO
    {
        public required string ComplaintText { get; set; }

        public ComplaintStatus Status { get; set; }

        public string ModeratorConclusion { get; set; } = string.Empty;
    }
}
