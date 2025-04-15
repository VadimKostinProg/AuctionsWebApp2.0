using BidMasterOnline.Application.DTO;
using BidMasterOnline.Application.Enums;

namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for managin tha auctions.
    /// </summary>
    public interface IAuctionsService
    {
        /// <summary>
        /// Gets list of auctions with applyed specifications.
        /// </summary>
        /// <param name="specifications">Specifications of sorting, pagination, filtering by category
        /// start price, current bid and status ty apply.</param>
        /// <returns>Auctions list with applyed specifications.</returns>
        Task<ListModel<AuctionDTO>> GetAuctionsListAsync(AuctionSpecificationsDTO specifications);

        /// <summary>
        /// Gets collection of the finished auctions with not yet confirmed payment and delivery options
        /// for authenticated customer.
        /// </summary>
        /// <param name="participant">Participant of the auction.</param>
        /// <returns>Collection IEnumerable of the auctions.</returns>
        Task<IEnumerable<AuctionDTO>> GetFinishedAuctionsWithNotConfirmedOptionsAsync(AuctionParticipant participant);

        /// <summary>
        /// Gets auctions information by its identifier.
        /// </summary>
        /// <param name="id">Identifier of auction to get information of.</param>
        /// <returns>Infromation of auction.</returns>
        Task<AuctionDTO> GetAuctionByIdAsync(Guid id);

        /// <summary>
        /// Gets auction detailed information by identifier.
        /// </summary>
        /// <param name="id">Identifier of auction to get information of.</param>
        /// <returns>Detailed infromation of auction.</returns>
        Task<AuctionDetailsDTO> GetAuctionDetailsByIdAsync(Guid id);

        /// <summary>
        /// Sets the score for the specified auction by user.
        /// </summary>
        /// <param name="request">Information of auction and score to set.</param>
        Task SetAuctionScoreAsync(SetAuctionScoreDTO request);

        /// <summary>
        /// Published auction for futher verification.
        /// </summary>
        /// <param name="request">Object with publishing auctions infromation.</param>
        Task PublishAuctionAsync(PublishAuctionDTO request);

        /// <summary>
        /// Cancels active auction.
        /// </summary>
        /// <param name="request">Information of auction to cancel and reason.</param>
        Task CancelAuctionAsync(CancelAuctionDTO request);

        /// <summary>
        /// Cancels auction of the authenticated user.
        /// </summary>
        /// <param name="auctionId">Identifier of auction to cancel.</param>
        Task CancelOwnAuctionAsync(Guid auctionId);

        /// <summary>
        /// Recovers canceled auction.
        /// </summary>
        /// <param name="auctionId">Identifier of auction to cancel.</param>
        /// <returns></returns>
        Task RecoverAuctionAsync(Guid auctionId);

        /// <summary>
        /// Sets the next bidder as winner of auction.
        /// </summary>
        /// <param name="auctionId">Identifier of auction to ser new winner.</param>
        Task SetNextWinnerOfAuctionAsync(Guid auctionId);
    }
}
