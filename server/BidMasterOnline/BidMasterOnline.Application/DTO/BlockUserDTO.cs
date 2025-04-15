using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for blocking user. (REQUEST)
    /// </summary>
    public class BlockUserDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string BlockingReason { get; set; }

        public int? Days { get; set; }
    }
}
