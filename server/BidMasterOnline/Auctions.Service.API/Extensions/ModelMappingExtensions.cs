using BidMasterOnline.Core.DTO;
using BidMasterOnline.Core.Extensions;
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
                BidAmountInterval = request.BidAmountInterval,
                AimPrice = request.AimPrice,
            };
        }

        public static DTO.Participant.AuctionRequestSummaryDTO ToParticipantSummaryDTO(this AuctionRequest entity)
        {
            return new DTO.Participant.AuctionRequestSummaryDTO
            {
                Id = entity.Id,
                LotTitle = entity.LotTitle,
                RequestedAuctionTime = TimeSpan.FromTicks(entity.RequestedAuctionTimeInTicks),
                StartPrice = entity.StartPrice,
                Status = entity.Status,
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
                StartPrice = entity.StartPrice,
                Category = entity.Category?.Name ?? string.Empty,
                Type = entity.Type?.Name ?? string.Empty,
                FinishMethod = entity.FinishMethod?.Name ?? string.Empty,
                RequestedStartTime = entity.RequestedStartTime,
                FinishTimeInterval = entity.FinishTimeIntervalInTicks == null
                    ? null
                    : TimeSpan.FromTicks(entity.FinishTimeIntervalInTicks.Value),
                BidAmountInterval = entity.BidAmountInterval,
                AimPrice = entity.AimPrice,
                Status = entity.Status,
                ReasonDeclined = entity.ReasonDeclined,
                ImageUrls = entity.Images?.Select(entity => entity.Url).ToList() ?? []
            };
        }

        public static DTO.Participant.AuctionSummaryDTO ToParticipantSummaryDTO(this Auction entity)
        {
            return new DTO.Participant.AuctionSummaryDTO
            {
                Id = entity.Id,
                LotTitle = entity.LotTitle,
                Category = entity.Category!.Name,
                StartTime = entity.StartTime,
                FinishTime = entity.FinishTime,
                StartPrice = entity.StartPrice,
                CurrentPrice = entity.CurrentPrice,
                AverageScore = entity.AverageScore,
                Auctioneer = entity.Auctioneer == null
                    ? null
                    : new BidMasterOnline.Core.DTO.UserSummaryDTO
                    {
                        UserId = entity.Auctioneer.Id,
                        Username = entity.Auctioneer.Username,
                        Email = entity.Auctioneer.Email
                    },
                ImageUrls = entity.Images?.Select(entity => entity.Url).ToList() ?? []
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
                BidAmountInterval = entity.BidAmountInterval,
                Status = entity.Status,
                StartTime = entity.StartTime,
                FinishTime = entity.FinishTime,
                AuctionTime = TimeSpan.FromTicks(entity.AuctionTimeInTicks),
                StartPrice = entity.StartPrice,
                CurrentPrice = entity.CurrentPrice,
                AverageScore = entity.AverageScore,
                FinishPrice = entity.FinishPrice,
                IsPaymentPerformed = entity.IsPaymentPerformed,
                IsDeliveryPerformed = entity.IsDeliveryPerformed,
                Auctioneer = entity.Auctioneer == null
                    ? null
                    : entity.Auctioneer.ToSummaryDTO(),
                Winner = entity.Winner == null
                    ? null
                    : entity.Winner.ToSummaryDTO(),
                ImageUrls = entity.Images?.Select(entity => entity.Url).ToList() ?? []
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

        public static DTO.Participant.AuctionCategoryDTO ToParticipantDTO(this AuctionCategory entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };

        public static DTO.Participant.AuctionTypeDTO ToParticipantDTO(this AuctionType entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };

        public static DTO.Participant.AuctionFinishMethodDTO ToParticipantDTO(this AuctionFinishMethod entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description
            };
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
                StartPrice = entity.StartPrice,
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
                AimPrice = entity.AimPrice,
                Status = entity.Status,
                StartPrice = entity.StartPrice,
                RequestedByUser = entity.RequestedByUser == null
                    ? null
                    : new UserSummaryDTO()
                    {
                        UserId = entity.RequestedByUser.Id,
                        Username = entity.RequestedByUser.Username,
                        Email = entity.RequestedByUser.Email,
                    },
                ReasonDeclined = entity.ReasonDeclined,
                ImageUrls = entity.Images?.Select(i => i.Url).ToList() ?? [],
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
                Category = entity.Category?.Name ?? string.Empty,
                Type = entity.Type?.Name ?? string.Empty,
                StartTime = entity.StartTime,
                FinishTime = entity.FinishTime,
                StartPrice = entity.StartPrice,
                CurrentPrice = entity.CurrentPrice,
                Status = entity.Status,
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
                IsPaymentPerformed = entity.IsPaymentPerformed,
                PaymentPerformedTime = entity.PaymentPerformedTime,
                IsDeliveryPerformed = entity.IsDeliveryPerformed,
                DeliveryPerformedTime = entity.DeliveryPerformedTime,
                DeliveryWaybill = entity.DeliveryWaybill,
                CancellationReason = entity.CancellationReason,
                Auctioneer = entity.Auctioneer == null
                    ? null
                    : new BidMasterOnline.Core.DTO.UserSummaryDTO
                    {
                        UserId = entity.Auctioneer.Id,
                        Username = entity.Auctioneer.Username,
                        Email = entity.Auctioneer.Email
                    },
                Winner = entity.Winner == null
                    ? null
                    : new BidMasterOnline.Core.DTO.UserSummaryDTO
                    {
                        UserId = entity.Winner.Id,
                        Username = entity.Winner.Username,
                        Email = entity.Winner.Email
                    },
                ImageUrls = entity.Images?.Select(entity => entity.Url).ToList() ?? [],
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
        
        public static DTO.Moderator.AuctionCategoryDTO ToModeratorDTO(this AuctionCategory entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy
            };

        public static DTO.Moderator.AuctionTypeDTO ToModeratorDTO(this AuctionType entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy
            };

        public static DTO.Moderator.AuctionFinishMethodDTO ToModeratorDTO(this AuctionFinishMethod entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy
            };
        #endregion
    }
}
