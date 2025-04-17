using BidMasterOnline.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BidMasterOnline.Domain.Entities
{
    public class Payment : EntityBase
    {
        public long PayerId { get; set; }

        public long RecipientId { get; set; }

        public long AuctionId { get; set; }

        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; }

        [MaxLength(100)]
        public string Provider { get; set; } = string.Empty;

        [MaxLength(100)]
        public required string TransactionId { get; set; }
    }
}
