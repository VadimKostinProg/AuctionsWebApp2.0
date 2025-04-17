using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class Delivery : EntityBase
    {
        public long AuctionId { get; set; }

        public long SellerId { get; set; }

        public long BuyerId { get; set; }

        [MaxLength(100)]
        public string Provider { get; set; } = string.Empty;

        [MaxLength(100)]
        public required string TrackingNumber { get; set; }

        public required string ShippingAddress { get; set; }
    }
}
