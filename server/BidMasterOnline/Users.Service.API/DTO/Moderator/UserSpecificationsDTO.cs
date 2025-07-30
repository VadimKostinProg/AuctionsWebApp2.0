using BidMasterOnline.Core.Enums;
using BidMasterOnline.Domain.Enums;

namespace Users.Service.API.DTO.Moderator
{
    public class UserSpecificationsDTO
    {
        public long? UserId { get; set; }

        public string? SearchTerm { get; set; }

        public UserStatus? Status { get; set; }

        public string? SortBy { get; set; }

        public SortDirection SortDirection { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
