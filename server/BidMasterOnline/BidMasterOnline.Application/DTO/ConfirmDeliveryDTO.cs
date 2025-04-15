using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for confirming delivery for auction by auctionist. (REQUEST)
    /// </summary>
    public class ConfirmDeliveryDTO
    {
        [Required(ErrorMessage = "Auction is required.")]
        public Guid AuctionId { get; set; }

        [Required(ErrorMessage = "WayBill is required.")]
        public string Waybill { get; set; }
    }
}
