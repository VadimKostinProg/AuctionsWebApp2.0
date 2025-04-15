namespace BidMasterOnline.Application.DTO
{
    /// <summary>
    /// DTO for payment and delivery options of auction. (RESPONSE)
    /// </summary>
    public class AuctionPaymentDeliveryOptionsDTO
    {
        public Guid Id { get; set; }

        public Guid AuctionId { get; set; }

        public Guid? WinnerId { get; set; }

        public string? Winner { get; set; }

        public bool ArePaymentOptionsSet { get; set; }

        public string? PaymentOptionsSetDateTime { get; set; }

        public string? IBAN { get; set; }

        public bool AreDeliveryOptionsSet { get; set; }

        public string? DeliveryOptionsSetDateTime { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public string? ZipCode { get; set; }

        public bool IsPaymentConfirmed { get; set; }

        public string? PaymentConfirmationDateTime { get; set; }

        public bool IsDeliveryConfirmed { get; set; }

        public string? DeliveryConfirmationDateTime { get; set; }

        public string? Waybill { get; set; }
    }
}
