using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;

namespace Feedbacks.Service.API.ServiceContracts.Moderator
{
    public interface IModeratorAuctionCommentsService
    {
        Task<ServiceResult<PaginatedList<ModeratorAuctionCommentDTO>>> GetAuctionCommentsAsync(long auctionId,
            PaginationRequestDTO pagination);

        Task<ServiceResult> DeleteCommentAsync(long commentId);
    }
}
