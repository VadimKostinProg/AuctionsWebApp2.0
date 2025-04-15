using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for rejection the auction. (REQUEST)
    /// </summary>
    public class RejectAuctionDTO
    {
        [Required]
        public Guid AuctionId { get; set; }

        [Required]
        public string RejectionReason { get; set; }
    }
}
