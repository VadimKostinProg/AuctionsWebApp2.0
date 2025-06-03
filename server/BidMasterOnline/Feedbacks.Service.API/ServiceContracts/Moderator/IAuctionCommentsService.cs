using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Moderator;

namespace Feedbacks.Service.API.ServiceContracts.Moderator
{
    public interface IAuctionCommentsService
    {
        Task<ServiceResult<PaginatedList<AuctionCommentDTO>>> GetAuctionCommentsAsync(long auctionId,
            PaginationRequestDTO pagination);

        Task<ServiceResult> DeleteCommentAsync(long commentId);
    }
}
