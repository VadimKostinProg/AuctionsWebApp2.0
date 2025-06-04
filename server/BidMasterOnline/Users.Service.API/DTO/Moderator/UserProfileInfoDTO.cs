using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Users.Service.API.DTO.Moderator
{
    public class UserProfileInfoDTO : BaseDTO
    {
        public required string Username { get; set; }

        public required string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public required string Email { get; set; }

        public double? AverageScore { get; set; }

        public UserStatus Status { get; set; }

        public string? ImageUrl { get; set; }

        public int TotalAuctions { get; set; }

        public int CompletedAuctions { get; set; }

        public int TotalWins { get; set; }
    }
}
