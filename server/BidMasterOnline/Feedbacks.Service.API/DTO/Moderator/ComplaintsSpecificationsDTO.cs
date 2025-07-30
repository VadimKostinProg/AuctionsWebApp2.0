using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class ComplaintsSpecificationsDTO : PaginationRequestDTO
    {
        public long? ModeratorId { get; set; } = null;

        public ComplaintType? Type { get; set; } = null;

        public ComplaintStatus? Status { get; set; } = null;
    }
}
