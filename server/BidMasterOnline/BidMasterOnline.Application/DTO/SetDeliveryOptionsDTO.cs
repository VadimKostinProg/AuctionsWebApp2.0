using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for setting the delivery options of the auction lot by winner. (REQUEST)
    /// </summary>
    public class SetDeliveryOptionsDTO
    {
        [Required(ErrorMessage = "Auction is required.")]
        public Guid AuctionId { get; set; }

        [Required(ErrorMessage = "Your country is required.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Your city is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Your ZIP code is required.")]
        public string ZipCode { get; set; }
    }
}
