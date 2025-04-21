using BidMasterOnline.Domain.Models.Entities;

namespace Auctions.Service.API.Extensions
{
    public static class ModelMappingExtensions
    {
        #region Participant
        public static AuctionRequest ToParticipantDomain(this DTO.Participant.PostAuctionRequestDTO request)
        {
            return new AuctionRequest
            {
                AuctionCategoryId = request.AuctionCategoryId,
                AuctionTypeId = request.AuctionTypeId,
                AuctionFinishMethodId = request.AuctionFinishMethodId,
                LotTitle = request.LotTitle,
                LotDescription = request.LotDescription,
                RequestedAuctionTimeInTicks = request.RequestedAuctionTime.Ticks,
                RequestedStartTime = request.RequestedStartTime,
                FinishTimeIntervalInTicks = request.FinishTimeInterval?.Ticks,
                StartPrice = request.StartPrice,
                BidAmountInterval = request.BidAmountInterval
            };
        }

        public static DTO.Participant.AuctionRequestSummaryDTO ToParticipantSummaryDTO(this AuctionRequest entity)
        {
            return new DTO.Participant.AuctionRequestSummaryDTO
            {
                Id = entity.Id,
                LotTitle = entity.LotTitle,
                RequestedAuctionTime = TimeSpan.FromTicks(entity.RequestedAuctionTimeInTicks),
                Status = entity.Status,
                Images = entity.Images?.Select(entity => entity.ToParticipantDTO()).ToList() ?? []
            };
        }

        public static DTO.Participant.AuctionRequestDTO ToParticipantDTO(this AuctionRequest entity)
        {
            return new DTO.Participant.AuctionRequestDTO
            {
                Id = entity.Id,
                LotTitle = entity.LotTitle,
                LotDescription = entity.LotDescription,
                RequestedAuctionTime = TimeSpan.FromTicks(entity.RequestedAuctionTimeInTicks),
                Category = entity.Category?.Name ?? string.Empty,
                Type = entity.Type?.Name ?? string.Empty,
                FinishMethod = entity.FinishMethod?.Name ?? string.Empty,
                RequestedStartTime = entity.RequestedStartTime,
                FinishTimeInterval = entity.FinishTimeIntervalInTicks == null
                    ? null
                    : TimeSpan.FromTicks(entity.FinishTimeIntervalInTicks.Value),
                BidAmountInterval = entity.BidAmountInterval,
                Status = entity.Status,
                ReasonDeclined = entity.ReasonDeclined,
                Images = entity.Images?.Select(entity => entity.ToParticipantDTO()).ToList() ?? []
            };
        }

        public static DTO.Participant.AuctionSummaryDTO ToParticipantSummaryDTO(this Auction entity)
        {
            return new DTO.Participant.AuctionSummaryDTO
            {
                Id = entity.Id,
                LotTitle = entity.LotTitle,
                StartTime = entity.StartTime,
                FinishTime = entity.FinishTime,
                CurrentPrice = entity.CurrentPrice,
                AverageScore = entity.AverageScore,
                Auctionist = entity.Auctionist == null
                    ? null
                    : new BidMasterOnline.Core.DTO.UserSummaryDTO
                    {
                        Id = entity.Auctionist.Id,
                        Username = entity.Auctionist.Username,
                        Email = entity.Auctionist.Email
                    },
                Images = entity.Images?.Select(entity => entity.ToParticipantDTO()).ToList() ?? []
            };
        }

        public static DTO.Participant.AuctionDTO ToParticipantDTO(this Auction entity)
        {
            return new DTO.Participant.AuctionDTO
            {
                Id = entity.Id,
                LotTitle = entity.LotTitle,
                LotDescription = entity.LotDescription,
                Category = entity.Category?.Name ?? string.Empty,
                Type = entity.Type?.Name ?? string.Empty,
                FinishMethod = entity.FinishMethod?.Name ?? string.Empty,
                FinishTimeInterval = entity.FinishTimeIntervalInTicks == null
                    ? null
                    : TimeSpan.FromTicks(entity.FinishTimeIntervalInTicks.Value),
                BidAmountInterval = entity.BidAmountInterval,
                Status = entity.Status,
                StartTime = entity.StartTime,
                FinishTime = entity.FinishTime,
                AuctionTime = TimeSpan.FromTicks(entity.AuctionTimeInTicks),
                StartPrice = entity.StartPrice,
                CurrentPrice = entity.CurrentPrice,
                AverageScore = entity.AverageScore,
                CancellationReason = entity.CancellationReason,
                Auctionist = entity.Auctionist == null
                    ? null
                    : new BidMasterOnline.Core.DTO.UserSummaryDTO
                    {
                        Id = entity.Auctionist.Id,
                        Username = entity.Auctionist.Username,
                        Email = entity.Auctionist.Email
                    },
                Winner = entity.Winner == null
                    ? null
                    : new BidMasterOnline.Core.DTO.UserSummaryDTO
                    {
                        Id = entity.Winner.Id,
                        Username = entity.Winner.Username,
                        Email = entity.Winner.Email
                    },
                Images = entity.Images?.Select(entity => entity.ToParticipantDTO()).ToList() ?? []
            };
        }

        public static DTO.Participant.AuctionImageDTO ToParticipantDTO(this AuctionImage entity)
        {
            return new DTO.Participant.AuctionImageDTO
            {
                PublicId = entity.PublicId,
                Url = entity.Url,
            };
        }
        #endregion

        #region Moderator
        public static DTO.Moderator.AuctionRequestSummaryDTO ToModeratorSummaryDTO(this AuctionRequest entity)
        {
            return new DTO.Moderator.AuctionRequestSummaryDTO
            {
                Id = entity.Id,
                LotTitle = entity.LotTitle,
                RequestedAuctionTime = TimeSpan.FromTicks(entity.RequestedAuctionTimeInTicks),
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy
            };
        }

        public static DTO.Moderator.AuctionRequestDTO ToModeratorDTO(this AuctionRequest entity)
        {
            return new DTO.Moderator.AuctionRequestDTO
            {
                Id = entity.Id,
                LotTitle = entity.LotTitle,
                LotDescription = entity.LotDescription,
                RequestedAuctionTime = TimeSpan.FromTicks(entity.RequestedAuctionTimeInTicks),
                Category = entity.Category?.Name ?? string.Empty,
                Type = entity.Type?.Name ?? string.Empty,
                FinishMethod = entity.FinishMethod?.Name ?? string.Empty,
                RequestedStartTime = entity.RequestedStartTime,
                FinishTimeInterval = entity.FinishTimeIntervalInTicks == null
                    ? null
                    : TimeSpan.FromTicks(entity.FinishTimeIntervalInTicks.Value),
                BidAmountInterval = entity.BidAmountInterval,
                Status = entity.Status,
                ReasonDeclined = entity.ReasonDeclined,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy
            };
        }

        public static DTO.Moderator.AuctionSummaryDTO ToModeratorSummaryDTO(this Auction entity)
        {
            return new DTO.Moderator.AuctionSummaryDTO
            {
                Id = entity.Id,
                LotTitle = entity.LotTitle,
                StartTime = entity.StartTime,
                FinishTime = entity.FinishTime,
                CurrentPrice = entity.CurrentPrice,
                AverageScore = entity.AverageScore,
                Auctionist = entity.Auctionist == null
                    ? null
                    : new BidMasterOnline.Core.DTO.UserSummaryDTO
                    {
                        Id = entity.Auctionist.Id,
                        Username = entity.Auctionist.Username,
                        Email = entity.Auctionist.Email
                    },
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy
            };
        }

        public static DTO.Moderator.AuctionDTO ToModeratorDTO(this Auction entity)
        {
            return new DTO.Moderator.AuctionDTO
            {
                Id = entity.Id,
                LotTitle = entity.LotTitle,
                LotDescription = entity.LotDescription,
                Category = entity.Category?.Name ?? string.Empty,
                Type = entity.Type?.Name ?? string.Empty,
                FinishMethod = entity.FinishMethod?.Name ?? string.Empty,
                FinishTimeInterval = entity.FinishTimeIntervalInTicks == null
                    ? null
                    : TimeSpan.FromTicks(entity.FinishTimeIntervalInTicks.Value),
                BidAmountInterval = entity.BidAmountInterval,
                Status = entity.Status,
                StartTime = entity.StartTime,
                FinishTime = entity.FinishTime,
                AuctionTime = TimeSpan.FromTicks(entity.AuctionTimeInTicks),
                StartPrice = entity.StartPrice,
                CurrentPrice = entity.CurrentPrice,
                AverageScore = entity.AverageScore,
                CancellationReason = entity.CancellationReason,
                Auctionist = entity.Auctionist == null
                    ? null
                    : new BidMasterOnline.Core.DTO.UserSummaryDTO
                    {
                        Id = entity.Auctionist.Id,
                        Username = entity.Auctionist.Username,
                        Email = entity.Auctionist.Email
                    },
                Winner = entity.Winner == null
                    ? null
                    : new BidMasterOnline.Core.DTO.UserSummaryDTO
                    {
                        Id = entity.Winner.Id,
                        Username = entity.Winner.Username,
                        Email = entity.Winner.Email
                    },
                Images = entity.Images?.Select(entity => entity.ToModeratorDTO()).ToList() ?? [],
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy
            };
        }

        public static DTO.Moderator.AuctionImageDTO ToModeratorDTO(this AuctionImage entity)
        {
            return new DTO.Moderator.AuctionImageDTO
            {
                Id = entity.Id,
                PublicId = entity.PublicId,
                Url = entity.Url,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy
            };
        }
        #endregion
    }
}
