using BidMasterOnline.Application.Enums;
using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for set new complaint. (REQUEST)
    /// </summary>
    public class SetComplaintDTO
    {
        [Required]
        public Guid AccusedUserId { get; set; }

        [Required]
        public Guid AuctionId { get; set; }

        public Guid? CommentId { get; set; }

        [Required]
        public ComplaintType ComplaintType { get; set; }

        [Required]
        [MaxLength(300)]
        public string ComplaintText { get; set; }
    }
}
