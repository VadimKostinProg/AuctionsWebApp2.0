using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for setting new bid. (REQUEST)
    /// </summary>
    public class SetBidDTO
    {
        [Required]
        public Guid AuctionId { get; set; }

        [Required]
        [Range(0, 10e7)]
        public decimal Amount { get; set; }
    }
}
