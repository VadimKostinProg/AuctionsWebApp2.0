using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Users.Service.API.DTO.Moderator
{
    public class UserProfileSummaryInfoDTO : BaseDTO
    {
        public required string Username { get; set; }

        public required string FullName { get; set; }

        public required string Email { get; set; }

        public UserStatus Status { get; set; }
    }
}
