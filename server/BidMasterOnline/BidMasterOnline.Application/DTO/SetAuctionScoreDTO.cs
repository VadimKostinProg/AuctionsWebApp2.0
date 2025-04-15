using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for setting the score for auction. (REQUEST)
    /// </summary>
    public class SetAuctionScoreDTO
    {
        [Required]
        public Guid AuctionId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Score { get; set; }
    }
}
