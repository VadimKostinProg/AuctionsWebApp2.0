using BidMasterOnline.Core.Extensions;
using BidMasterOnline.Domain.Models.Entities;

namespace Bids.Service.API.Extensions
{
    public static class ModelMappingExtensions
    {
        #region Participant
        public static DTO.Participant.AuctionBidDTO ToParticipantAuctionBidDTO(this Bid entity)
            => new()
            {
                AuctionId = entity.AuctionId,
                Amount = entity.Amount,
                Time = entity.CreatedAt,
                Bidder = entity.Bidder!.ToSummaryDTO()
            };

        public static DTO.Participant.UserBidDTO ToParticipantUserBidDTO(this Bid entity)
            => new()
            {
                BidderId = entity.BidderId,
                AuctionId = entity.AuctionId,
                Amount = entity.Amount,
                Time = entity.CreatedAt,
                AuctionName = entity.Auction!.LotTitle
            };


        public static Bid ToDomain(this DTO.Participant.PostBidDTO requestDTO)
            => new()
            {
                AuctionId = requestDTO.AuctionId,
                Amount = requestDTO.Amount,
            };
        #endregion

        #region Moderator
        public static DTO.Moderator.AuctionBidDTO ToModeratorAuctionBidDTO(this Bid entity)
            => new()
            {
                Id = entity.Id,
                AuctionId = entity.AuctionId,
                BidderId = entity.BidderId,
                Amount = entity.Amount,
                BidderUsername = entity.Bidder?.Username ?? string.Empty,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.ModifiedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                Deleted = entity.Deleted
            };
        public static DTO.Moderator.UserBidDTO ToModeratorUserBidDTO(this Bid entity)
            => new()
            {
                Id = entity.Id,
                AuctionId = entity.AuctionId,
                BidderId = entity.BidderId,
                Amount = entity.Amount,
                AuctionName = entity.Auction?.LotTitle ?? string.Empty,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.ModifiedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                Deleted = entity.Deleted
            };

        #endregion
    }
}
