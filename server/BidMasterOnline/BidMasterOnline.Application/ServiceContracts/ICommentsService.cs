using BidMasterOnline.Application.DTO;

namespace BidMasterOnline.Application.ServiceContracts
{
    /// <summary>
    /// Service for managing comments.
    /// </summary>
    public interface ICommentsService
    {
        /// <summary>
        /// Gets all comments for the specified auction.
        /// </summary>
        /// <param name="id">Identifier of the auction to get comments.</param>
        /// <returns>Collection IEnumerable of comments for the specified auction.</returns>
        Task<IEnumerable<CommentDTO>> GetCommentsForAuctionAsync(Guid id);

        /// <summary>
        /// Gets comment by it`s identifier.
        /// </summary>
        /// <param name="id">Identifier of comment to get.</param>
        /// <returns>Comment with passed identifier.</returns>
        Task<CommentDTO> GetCommentByIdAsync(Guid id);

        /// <summary>
        /// Sets new comment of user for auction.
        /// </summary>
        /// <param name="comment">Comment to set.</param>
        Task SetNewCommentAsync(SetCommentDTO comment);

        /// <summary>
        /// Deletes comment with passed identifier by technical support specialist.
        /// </summary>
        /// <param name="id">Identifier of comment to delete.</param>
        Task DeleteCommentAsync(Guid id);

        /// <summary>
        /// Deletes own comment of the user.
        /// </summary>
        /// <param name="id">Identifier of comment to delete.</param>
        /// <returns></returns>
        Task DeleteOwnCommentAsync(Guid id);
    }
}
