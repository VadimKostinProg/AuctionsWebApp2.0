namespace Auctions.Service.API.DTO.Participant
{
    public class SetDeliveryWaybillDTO
    {
        public long AuctionId { get; set; }

        public required string Waybill { get; set; }
    }
}
