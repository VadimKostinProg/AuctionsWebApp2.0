using System.ComponentModel.DataAnnotations;

namespace Feedbacks.Service.API.DTO.Moderator
{
    public class CompleteComplaintDTO
    {
        public long ComplaintId { get; set; }

        [MaxLength(10000)]
        public required string ModeratorConclusion { get; set; }
    }
}
