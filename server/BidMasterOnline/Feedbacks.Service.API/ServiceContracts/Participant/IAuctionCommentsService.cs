using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;

namespace Feedbacks.Service.API.ServiceContracts.Participant
{
    public interface IAuctionCommentsService
    {
        Task<ServiceResult> PostAuctionCommentAsync(PostCommentDTO comment);

        Task<ServiceResult<PaginatedList<AuctionCommentDTO>>> GetAuctionCommentsAsync(long auctionId, 
            PaginationRequestDTO pagination);

        Task<ServiceResult> DeleteCommentAsync(long commentId);
    }
}
