using BidMasterOnline.Domain.Models.Entities;

namespace Users.Service.API.Extensions
{
    public static class ModelMappingExtensions
    {
        public static DTO.Participant.UserProfileInfoDTO ToUserProfileDTO(this User entity)
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

        public static DTO.Participant.ExpandedUserProfileInfoDTO ToExpandedUserProfileDTO(this User entity)
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
    }
}
