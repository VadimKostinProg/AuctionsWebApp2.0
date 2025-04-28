using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class ModeratorComplaintDTO : BaseDTO
    {
        public long AccusingUserId { get; set; }

        public long AccusedUserId { get; set; }

        public long? AccusedAuctionId { get; set; }

        public long? AccusedCommentId { get; set; }

        public long? AccusedUserFeedbackId { get; set; }

        public long? ModeratorId { get; set; }

        public required string Title { get; set; }

        public required string ComplaintText { get; set; }

        public ComplaintType Type { get; set; }

        public ComplaintStatus Status { get; set; }

        public string? ModeratorConclusion { get; set; }

        public string? ModeratorName { get; set; }

        public string AccusingUserName { get; set; } = string.Empty;

        public string AccusedUsername { get; set; } = string.Empty;

        public string? AccusedAuctionName { get; set; }

        public ModeratorAuctionCommentDTO? AccusedComment { get; set; }

        public ModeratorUserFeedbackDTO? AccusedUserFeedback { get; set; }
    }
}
