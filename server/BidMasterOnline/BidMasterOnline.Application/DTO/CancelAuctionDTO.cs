using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for canceling the auction. (REQUEST)
    /// </summary>
    public class CancelAuctionDTO
    {
        [Required]
        public Guid AuctionId { get; set; }

        [Required]
        public string CancelationReason { get; set; }
    }
}
