using System.ComponentModel.DataAnnotations;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class ModeratorCompleteComplaintDTO
    {
        public long ComplaintId { get; set; }

        [MaxLength(10000)]
        public required string ModeratorConclusion { get; set; }
    }
}
