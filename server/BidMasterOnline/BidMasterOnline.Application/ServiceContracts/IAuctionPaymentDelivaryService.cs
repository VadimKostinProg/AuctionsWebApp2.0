using BidMasterOnline.Application.DTO;

namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing the payment and delivery of the auctions.
    /// </summary>
    public interface IAuctionPaymentDelivaryService
    {
        /// <summary>
        /// Gets sell and delivery options of the auction.
        /// </summary>
        /// <param name="auctionId">Identifier of auction to get sell an delivery options of.</param>
        /// <returns>Auctions sell and delivery options.</returns>
        Task<AuctionPaymentDeliveryOptionsDTO> GetPaymentDeliveryOptionsForAuctionByIdAsync(Guid auctionId);

        /// <summary>
        /// Sets the payment options of the auction.
        /// </summary>
        /// <param name="auctionId">Identifier of auction to set payment options.</param>
        Task SetPaymentOptionsForAuctionAsync(SetPaymentOptionsDTO request);

        /// <summary>
        /// Sets the delivery options options of the auction.
        /// </summary>
        /// <param name="auctionId">Identifier of auction to set delivery options.</param>
        Task SetDeliveryOptionsForAuctionAsync(SetDeliveryOptionsDTO request);

        /// <summary>
        /// Confirms payment for the specified auction.
        /// </summary>
        /// <param name="auctionId">Identifier of the auction to confirm payment.</param>
        Task ConfirmPaymentForAuctionAsync(Guid auctionId);

        /// <summary>
        /// Confirms delivery of the auction lot.
        /// </summary>
        /// <param name="request">Information of auction and waybill.</param>
        Task ConfirmDeliveyForAuctionAsync(ConfirmDeliveryDTO request);
    }
}
