using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for setting new comment. (REQUEST)
    /// </summary>
    public class SetCommentDTO
    {
        [Required]
        public Guid AuctionId { get; set; }

        [Required]
        [MaxLength(300)]
        public string CommentText { get; set; }
    }
}
