using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for setting auction payment options by the auctionist. (REQUEST)
    /// </summary>
    public class SetPaymentOptionsDTO
    {
        [Required(ErrorMessage = "Auction is required.")]
        public Guid AuctionId { get; set; }

        [Required(ErrorMessage = "IBAN for apllying the payment is reqiured.")]
        public string IBAN { get; set; }
    }
}
