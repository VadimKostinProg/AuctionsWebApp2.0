using BidMasterOnline.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Feedbacks.Service.API.DTO.Participant
{
    public class ParticipantPostComplaintDTO
    {
        public long AccusedUserId { get; set; }

        public long? AccusedAuctionId { get; set; }

        public long? AccusedCommentId { get; set; }

        public long? AccusedUserFeedbackId { get; set; }

        [MaxLength(10000)]
        public required string ComplaintText { get; set; }

        public ComplaintType Type { get; set; }
    }
}
