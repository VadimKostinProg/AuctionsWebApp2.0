using BidMasterOnline.Application.DTO;

namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for auctions verification.
    /// </summary>
    public interface IAuctionVerificationService
    {
        /// <summary>
        /// Gets not approved auctions with applying specifications.
        /// </summary>
        /// <param name="specifications">Specifications of sorting and pagination to apply.</param>
        /// <returns>Auctions list with applyed specifications.</returns>
        Task<ListModel<AuctionDTO>> GetNotApprovedAuctionsListAsync(SpecificationsDTO specifications);

        /// <summary>
        /// Gets not approved auctions details by its identifier.
        /// </summary>
        /// <param name="id">Identifier of the auction.</param>
        /// <returns>Details of the not approved auction with passed identifier.</returns>
        Task<AuctionDetailsDTO> GetNotApprovedAuctionDetailsByIdAsync(Guid id);

        /// <summary>
        /// Approves the specified auction.
        /// </summary>
        /// <param name="auctionId">Identifier of auction to approve.</param>
        Task ApproveAuctionAsync(Guid auctionId);

        /// <summary>
        /// Rejects the specified auction.
        /// </summary>
        /// <param name="request">Information of the auction and rejection reason.</param>
        Task RejectAuctionAsync(RejectAuctionDTO request);
    }
}
