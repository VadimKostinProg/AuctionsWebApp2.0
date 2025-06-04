using BidMasterOnline.Domain.Models.Entities;

namespace Users.Service.API.Extensions
{
    public static class ModelMappingExtensions
    {
        #region Participant
        public static DTO.Participant.UserProfileInfoDTO ToParticipantUserProfileDTO(this User entity)
            => new()
            {
                Id = entity.Id,
                Username = entity.Username,
                FullName = entity.FullName,
                DateOfBirth = entity.DateOfBirth,
                Email = entity.Email,
                AverageScore = entity.AverageScore,
                Status = entity.Status,
                ImageUrl = entity.ImageUrl,
                TotalAuctions = entity.TotalAuctions,
                CompletedAuctions = entity.CompletedAuctions,
                TotalWins = entity.TotalWins,
            };

        public static DTO.Participant.ExpandedUserProfileInfoDTO ToExpandedParticipantUserProfileDTO(this User entity)
            => new()
            {
                Id = entity.Id,
                Username = entity.Username,
                FullName = entity.FullName,
                DateOfBirth = entity.DateOfBirth,
                Email = entity.Email,
                AverageScore = entity.AverageScore,
                Status = entity.Status,
                ImageUrl = entity.ImageUrl,
                IsEmailConfirmed = entity.IsEmailConfirmed,
                IsPaymentMethodAttached = entity.IsPaymentMethodAttached,
                UnblockDateTime = entity.UnblockDateTime,
                TotalAuctions = entity.TotalAuctions,
                CompletedAuctions = entity.CompletedAuctions,
                TotalWins = entity.TotalWins
            };
        #endregion

        #region Moderator
        public static DTO.Moderator.UserProfileSummaryInfoDTO ToModeratorUserProfileSummaryDTO(this User entity)
            => new()
            {
                Id = entity.Id,
                Username = entity.Username,
                FullName = entity.FullName,
                DateOfBirth = entity.DateOfBirth,
                Email = entity.Email,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy,
                Deleted = entity.Deleted
            };

        public static DTO.Moderator.UserProfileInfoDTO ToModeratorUserProfileDTO(this User entity)
            => new()
            {
                Id = entity.Id,
                Username = entity.Username,
                FullName = entity.FullName,
                DateOfBirth = entity.DateOfBirth,
                Email = entity.Email,
                AverageScore = entity.AverageScore,
                Status = entity.Status,
                Role = entity.Role!.Name,
                ImageUrl = entity.ImageUrl,
                IsEmailConfirmed = entity.IsEmailConfirmed,
                IsPaymentMethodAttached = entity.IsPaymentMethodAttached,
                UnblockDateTime = entity.UnblockDateTime,
                TotalAuctions = entity.TotalAuctions,
                CompletedAuctions = entity.CompletedAuctions,
                TotalWins = entity.TotalWins,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy,
                Deleted = entity.Deleted
            };
        #endregion
    }
}
