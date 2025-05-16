using BidMasterOnline.Core.DTO;
using Feedbacks.Service.API.DTO.Participant;

namespace Feedbacks.Service.API.ServiceContracts.Participant
{
    public interface IParticipantAuctionCommentsService
    {
        Task<ServiceResult> PostAuctionCommentAsync(ParticipantPostCommentDTO comment);

        Task<ServiceResult<PaginatedList<ParticipantAuctionCommentDTO>>> GetAuctionCommentsAsync(long auctionId, 
            PaginationRequestDTO pagination);

        Task<ServiceResult> DeleteCommentAsync(long commentId);
    }
}
