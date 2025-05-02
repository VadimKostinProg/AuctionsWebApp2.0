using BidMasterOnline.Core.DTO;
using BidMasterOnline.Domain.Enums;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class ModeratorSupportTicketsSpecificationsDTO : PaginationRequestDTO
    {
        public long? ModeratorId { get; set; } = null;

        public SupportTicketStatus? Status { get; set; } = null;
    }
}
