using BidMasterOnline.Application.DTO;

namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing the auction bids.
    /// </summary>
    public interface IBidsService
    {
        /// <summary>
        /// Gets all bids of the specified auction.
        /// </summary>
        /// <param name="auctionId">Identifier of the auction to get bids.</param>
        /// <param name="specifications">Specifications for pagination bids.</param>
        /// <returns>List of the bids for auction.</returns>
        Task<ListModel<BidDTO>> GetBidsListForAuctionAsync(Guid auctionId, SpecificationsDTO specifications);

        /// <summary>
        /// Gets all bids of the specified user.
        /// </summary>
        /// <param name="userId">Identifier of the user to get bids.</param>
        /// <param name="specifications">Specifications for pagination bids.</param>
        /// <returns>List of the bids for user.</returns>
        Task<ListModel<BidDTO>> GetBidsListForUserAsync(Guid userId, SpecificationsDTO specifications);

        /// <summary>
        /// Sets new bid of the specified user to the specified auction.
        /// </summary>
        /// <param name="bid">Bid information to set.</param>
        Task SetBidAsync(SetBidDTO bid);
    }
}
