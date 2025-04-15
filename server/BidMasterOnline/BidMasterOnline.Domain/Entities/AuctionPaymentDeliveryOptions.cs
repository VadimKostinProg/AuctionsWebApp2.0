using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace BidMasterOnline.Domain.Entities
{
    public class AuctionPaymentDeliveryOptions : EntityBase
    {
        public Guid AuctionId { get; set; }

        public Guid? WinnerId { get; set; }

        public bool ArePaymentOptionsSet { get; set; }

        public DateTime? PaymentOptionsSetDateTime { get; set; }

        public string? IBAN { get; set; }

        public bool AreDeliveryOptionsSet { get; set; }

        public DateTime? DeliveryOptionsSetDateTime { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public string? ZipCode { get; set; }

        public bool IsPaymentConfirmed { get; set; }

        public DateTime? PaymentConfirmationDateTime { get; set; }

        public bool IsDeliveryConfirmed { get; set; }

        public DateTime? DeliveryConfirmationDateTime { get; set; }

        public string? Waybill { get; set; }

        [ForeignKey(nameof(WinnerId))]
        public User? Winner { get; set; }
    }
}
