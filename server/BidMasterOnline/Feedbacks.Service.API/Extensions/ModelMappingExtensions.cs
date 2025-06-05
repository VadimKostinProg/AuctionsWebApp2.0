using BidMasterOnline.Domain.Enums;
using BidMasterOnline.Domain.Models.Entities;

namespace Feedbacks.Service.API.Extensions
{
    public static class ModelMappingExtensions
    {
        #region Participant
        public static DTO.Participant.AuctionCommentDTO ToParticipantDTO(this AuctionComment entity)
            => new()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                AuctionId = entity.Id,
                Score = entity.Score,
                CommentText = entity.CommentText,
                CreatedAt = entity.CreatedAt,
                Username = entity.User!.Username,
            };

        public static DTO.Participant.SummaryComplaintDTO ToParticipantSummaryDTO(this Complaint entity)
            => new()
            {
                Id = entity.Id,
                Title = entity.Type switch
                {
                    ComplaintType.ComplaintOnAuctionContent => $"Complaint on auciton content",
                    ComplaintType.ComplaintOnUserBehaviour => $"Complaint on user behaviour",
                    ComplaintType.ComplaintOnAuctionComment => $"Complaint on auction comment",
                    ComplaintType.ComplaintOnUserFeedback => $"Complaint on user feedback",
                    _ => string.Empty
                },
                CreatedAt = entity.CreatedAt,
                Status = entity.Status
            };

        public static DTO.Participant.ComplaintDTO ToParticipantDTO(this Complaint entity)
            => new()
            {
                Id = entity.Id,
                AccusedUserId = entity.AccusedUserId,
                AccusedAuctionId = entity.AccusedAuctionId,
                AccusedCommentId = entity.AccusedCommentId,
                AccusedUserFeedbackId = entity.AccusedUserFeedbackId,
                Title = entity.Type switch
                {
                    ComplaintType.ComplaintOnAuctionContent => $"Complaint on auciton content",
                    ComplaintType.ComplaintOnUserBehaviour => $"Complaint on user behaviour",
                    ComplaintType.ComplaintOnAuctionComment => $"Complaint on auction comment",
                    ComplaintType.ComplaintOnUserFeedback => $"Complaint on user feedback",
                    _ => string.Empty
                },
                ComplaintText = entity.ComplaintText,
                ModeratorConclusion = entity.ModeratorConclusion,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt
            };

        public static DTO.Participant.SummarySupportTicketDTO ToParticipantSummaryDTO(this SupportTicket entity)
            => new()
            {
                Id = entity.Id,
                Title = entity.Title,
                CreatedAt = entity.CreatedAt,
                Status = entity.Status
            };

        public static DTO.Participant.SupportTicketDTO ToParticipantDTO(this SupportTicket entity)
            => new()
            {
                Id = entity.Id,
                Title = entity.Title,
                Text = entity.Text,
                Status = entity.Status,
                ModeratorComment = entity.ModeratorComment,
                CreatedAt = entity.CreatedAt
            };

        public static DTO.Participant.UserFeedbackDTO ToParticipantDTO(this UserFeedback entity)
            => new()
            {
                Id = entity.Id,
                FromUserId = entity.FromUserId,
                FromUsername = entity.FromUser!.Username,
                Score = entity.Score,
                Comment = entity.Comment,
                CreatedAt = entity.CreatedAt
            };

        public static AuctionComment ToDomain(this DTO.Participant.PostCommentDTO dto)
            => new()
            {
                AuctionId = dto.AuctionId,
                Score = dto.Score,
                CommentText = dto.CommentText
            };

        public static Complaint ToDomain(this DTO.Participant.PostComplaintDTO dto)
            => new()
            {
                AccusedUserId = dto.AccusedUserId,
                AccusedAuctionId = dto.AccusedAuctionId,
                AccusedCommentId = dto.AccusedCommentId,
                AccusedUserFeedbackId = dto.AccusedUserFeedbackId,
                ComplaintText = dto.ComplaintText,
                Type = dto.Type
            };

        public static SupportTicket ToDomain(this DTO.Participant.PostSupportTicketDTO dto)
            => new()
            {
                Title = dto.Title,
                Text = dto.Text
            };

        public static UserFeedback ToDomain(this DTO.Participant.PostUserFeedbackDTO dto)
            => new()
            {
                ToUserId = dto.ToUserId,
                Score = dto.Score,
                Comment = dto.Comment,
            };
        #endregion

        #region Moderator
        public static DTO.Moderator.AuctionCommentDTO ToModeratorDTO(this AuctionComment entity)
            => new()
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy,
                UserId = entity.UserId,
                AuctionId = entity.AuctionId,
                Score = entity.Score,
                CommentText = entity.CommentText,
                Username = entity.User!.Username
            };

        public static DTO.Moderator.SummaryComplaintDTO ToModeratorSummaryDTO(this Complaint entity)
            => new()
            {
                Id = entity.Id,
                ModeratorId = entity.ModeratorId,
                AccusingUserId = entity.AccusingUserId,
                Title = entity.Type switch
                {
                    ComplaintType.ComplaintOnAuctionContent => $"Complaint on auciton content",
                    ComplaintType.ComplaintOnUserBehaviour => $"Complaint on user behaviour",
                    ComplaintType.ComplaintOnAuctionComment => $"Complaint on auction comment",
                    ComplaintType.ComplaintOnUserFeedback => $"Complaint on user feedback",
                    _ => string.Empty
                },
                Status = entity.Status,
                AccusingUsername = entity.AccusingUser!.FullName,
                ModeratorName = entity.Moderator?.FullName,
                SubmittedTime = entity.CreatedAt,
            };

        public static DTO.Moderator.ComplaintDTO ToModeratorDTO(this Complaint entity)
            => new()
            {
                Id = entity.Id,
                ModeratorId = entity.ModeratorId,
                AccusingUserId = entity.AccusingUserId,
                AccusedUserId = entity.AccusedUserId,
                AccusedAuctionId = entity.AccusedAuctionId,
                AccusedCommentId = entity.AccusedCommentId,
                AccusedUserFeedbackId = entity.AccusedUserFeedbackId,
                AccusingUserName = entity.AccusingUser!.Username,
                AccusedUsername = entity.AccusedUser!.Username,
                AccusedAuctionName = entity.AccusedAuction?.LotTitle,
                AccusedComment = entity.AccusedComment?.ToModeratorDTO(),
                AccusedUserFeedback = entity.AccusedUserFeedback?.ToModeratorDTO(),
                Title = entity.Type switch
                {
                    ComplaintType.ComplaintOnAuctionContent => $"Complaint on auciton content",
                    ComplaintType.ComplaintOnUserBehaviour => $"Complaint on user behaviour",
                    ComplaintType.ComplaintOnAuctionComment => $"Complaint on auction comment",
                    ComplaintType.ComplaintOnUserFeedback => $"Complaint on user feedback",
                    _ => string.Empty
                },
                ComplaintText = entity.ComplaintText,
                ModeratorConclusion = entity.ModeratorConclusion,
                Type = entity.Type,
                Status = entity.Status,
                ModeratorName = entity.Moderator?.FullName,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy,
            };

        public static DTO.Moderator.SupportTicketSummaryDTO ToModeratorSummaryDTO(this SupportTicket entity)
            => new()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                ModeratorId = entity.ModeratorId,
                Title = entity.Title,
                Status = entity.Status,
                SubmittedTime = entity.CreatedAt,
                SubmittedUsername = entity.User!.Username,
                ModeratorName = entity.Moderator?.FullName,
            };

        public static DTO.Moderator.SupportTicketDTO ToModeratorDTO(this SupportTicket entity)
            => new()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                ModeratorId = entity.ModeratorId,
                Title = entity.Title,
                Text = entity.Text,
                Status = entity.Status,
                ModeratorComment = entity.ModeratorComment,
                SubmitUsername = entity.User!.Username,
                ModeratorName = entity.Moderator?.FullName,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy,
            };

        public static DTO.Moderator.UserFeedbackDTO ToModeratorDTO(this UserFeedback entity)
            => new()
            {
                Id = entity.Id,
                FromUserId = entity.FromUserId,
                ToUserId = entity.ToUserId,
                Score = entity.Score,
                Comment = entity.Comment,
                FromUsername = entity.FromUser!.Username,
                ToUsername = entity.ToUser!.Username,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy,
            };
        #endregion
    }
}
