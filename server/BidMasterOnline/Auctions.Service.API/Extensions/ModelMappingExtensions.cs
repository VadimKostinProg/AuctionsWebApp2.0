using Auctions.Service.API.DTO;
using BidMasterOnline.Domain.Entities;

namespace Auctions.Service.API.Extensions
{
    public static class ModelMappingExtensions
    {
        public static AuctionRequest ToDomain(this PostAuctionRequestDTO request)
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

        public static AuctionRequestSummaryDTO ToSummaryDTO(this AuctionRequest entity)
        {
            return new AuctionRequestSummaryDTO
            {
                Id = entity.Id,
                LotTitle = entity.LotTitle,
                RequestedAuctionTime = TimeSpan.FromTicks(entity.RequestedAuctionTimeInTicks),
                Status = entity.Status,
                Images = entity.Images?.Select(entity => entity.ToDTO()).ToList() ?? []
            };
        }

        public static AuctionRequestDTO ToDTO(this AuctionRequest entity)
        {
            return new AuctionRequestDTO
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
                Images = entity.Images?.Select(entity => entity.ToDTO()).ToList() ?? []
            };
        }

        public static AuctionSummaryDTO ToSummaryDTO(this Auction entity)
        {
            return new AuctionSummaryDTO
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
                Images = entity.Images?.Select(entity => entity.ToDTO()).ToList() ?? []
            };
        }

        public static AuctionDTO ToDTO(this Auction entity)
        {
            return new AuctionDTO
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
                Images = entity.Images?.Select(entity => entity.ToDTO()).ToList() ?? []
            };
        }

        public static AuctionImageDTO ToDTO(this AuctionImage entity)
        {
            return new AuctionImageDTO
            {
                PublicId = entity.PublicId,
                Url = entity.Url,
            };
        }
    }
}
